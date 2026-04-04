// Option and Result types exist in F# already!

type Cat = { Name: string }
type Dog = { Name: string }

type Pet = Cat of Cat | Dog of Dog

type CatError = TooCute | Lazy

let failableCat meow =
  match meow with
  | "meow" -> Ok (Cat { Name = meow })
  | _ -> Error CatError.TooCute

let result1 = failableCat "meow"
let result2: Result<Pet, CatError> = failableCat "rawr"
let result3 = failableCat "rawr" |> Result.defaultValue (Pet.Cat { Name = "qtπ" })

printfn $"{result2.ToString()}"
printfn $"{result3}"
