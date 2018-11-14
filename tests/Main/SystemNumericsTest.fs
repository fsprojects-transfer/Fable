﻿module Fable.Tests.SystemNumerics

open System
open Util.Testing
#if FABLE_COMPILER
open Fable.Core.JsInterop
#endif



let tests =
  testList "System.Numerics" [
    

      // vector3
      
      testCase "Vector3.Zero works" <| fun () ->
            let v1 = System.Numerics.Vector3(0.f, 0.f, 0.f)
            v1 |> equal System.Numerics.Vector3.Zero
            [| v1.X; v1.Y; v1.Z |] |> equal [| 0.f; 0.f; 0.f |]
            

      testCase "Vector3.One works" <| fun () ->
            let v1 = System.Numerics.Vector3(1.f, 1.f, 1.f)
            v1 |> equal System.Numerics.Vector3.One
            [| v1.X; v1.Y; v1.Z |] |> equal [| 1.f; 1.f; 1.f |]
    

      // Quaternion
      
      testCase "Quaternion.Identity works" <| fun () ->
            let q1 = System.Numerics.Quaternion(0.f, 0.f, 0.f, 1.f)
            q1 |> equal System.Numerics.Quaternion.Identity
            [| q1.X; q1.Y; q1.Z; q1.W |] |> equal [| 0.f; 0.f; 0.f; 1.f |]
]