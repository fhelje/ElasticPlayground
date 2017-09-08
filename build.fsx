#I @"packages/build/FAKE/tools"
#r @"FakeLib.dll"

open System
open Fake.Core
open Fake.Core.Trace
open Fake.Core.TargetOperators
open Fake.DotNetCli

Target.Create "Clean" ( fun _ ->
    trace "Clean"
)

Target.Create "Build" ( fun _ ->
    trace "Build with dotnet cli"
    Build (fun p -> 
       { p with             
            Configuration = "Release" })    
)

Target.Create "Test" ( fun _ ->
    trace "Test"
    Test (fun p -> 
       { p with             
            Project = "ElasticPlayground.Tests"
            Configuration = "Release" })    
)

"Clean"
    ==> "Build"
    ==> "Test"

Target.RunOrDefault "Test"