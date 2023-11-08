a simple developer console

### example

make command on UnityEngine.Object

```cs
[CommandObject]
public class Foo : MonoBehaviour
{
    [Command]
    public new string name = "default";
    
    [Command]
    public void Print(int id)
    {
        $"I am {GetHashCode()} and {id}".Dump();
    }

}
```

input command in console:
```
get_foo_name
set_foo_name slipknot
foo_print 42
```

you can custom the command name:
```cs
[CommandObject]
public class Foo : MonoBehaviour
{
    [Command("id")]
    public new string name = "default";
    
    [Command("printfoo")]
    public void Print(int id)
    {
        $"I am {GetHashCode()} and {id}".Dump();
    }

}
```

```
get_id
set_id darkous
printfoo 24
```

make a command by extend ConsoleCommand:
```cs
[CommandType]
class AddCommand : ConsoleCommand
{
    public override string Name => "add"
    public override string SuccessMessage => ""
    public override string ErrorMessage => "add error"

    protected void OnExecute(int a, int b)
    {
        (a + b).Dump();
    }
}
```

the command will invoke the method named OnExecute
if you add `CommandTypeAttribute`, console will automatically add it


