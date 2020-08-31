module Fable.Transforms.BabelPrinter

open System
open Fable.Core
open Fable.AST.Babel

type SourceMapGenerator =
    abstract AddMapping: source: string
        * originalLine: int
        * originalColumn: int
        * generatedLine: int
        * generatedColumn: int
        * ?name: string
        -> unit

type FilePrinter(path: string, map: SourceMapGenerator) =
    // TODO: We can make this configurable later
    let indentSpaces = "    "
    let stream = new IO.StreamWriter(path)
    let builder = Text.StringBuilder()
    let mutable indent = 0
    let mutable line = 1
    let mutable column = 0

    member _.Flush(): Async<unit> =
        async {
            do! stream.WriteAsync(builder.ToString()) |> Async.AwaitTask
            builder.Clear() |> ignore
        }

    interface IDisposable with
        member _.Dispose() = stream.Dispose()

    interface Printer with
        member _.Line = line
        member _.Column = column

        member _.PushIndentation() =
            indent <- indent + 1

        member _.PopIndentation() =
            if indent > 0 then indent <- indent - 1

        member _.Print(str, loc) =
            match loc with
            | None -> ()
            | Some loc ->
                map.AddMapping(source=path,
                    originalLine = loc.start.line,
                    originalColumn = loc.start.column,
                    generatedLine = line,
                    generatedColumn = column,
                    ?name = loc.identifierName)

            if column = 0 then
                let indent = String.replicate indent indentSpaces
                builder.Append(indent) |> ignore
                column <- indent.Length

            builder.Append(str) |> ignore
            column <- column + str.Length

        member _.PrintNewLine() =
            builder.AppendLine() |> ignore
            builder.Append(String.replicate indent indentSpaces) |> ignore
            line <- line + 1
            column <- 0

let run (program: Program): Async<unit> =
    // TODO: Dummy interface until we have a dotnet port of SourceMapGenerator
    // https://github.com/mozilla/source-map#with-sourcemapgenerator-low-level-api
    let map =
        { new SourceMapGenerator with
            member _.AddMapping(_,_,_,_,_,_) = () }

    async {
        use printer = new FilePrinter(program.FileName + ".js", map)
        for decl in program.Body do
            match decl with
            | U2.Case1 statement -> statement.Print(printer)
            | U2.Case2 moduleDecl -> moduleDecl.Print(printer)
            do! printer.Flush()
    }