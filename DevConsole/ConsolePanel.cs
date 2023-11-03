using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtil.DevConsole;

public class ConsolePanel : MonoBehaviour
{

    [SerializeField]
    private TMP_Text history;

    [SerializeField]
    private Button btn;

    [SerializeField]
    private TMP_InputField inputField;

    private WrapperConsole console;

    void Awake()
    {
        console = new WrapperConsole(history, btn, inputField);
    }

    void Start()
    {
        console.AddCommand(new HelloCommand(), new ErrorCommand(), new PrintCommand(), new AddCommand());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            console.SendCommand();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            string last = console.GetLastCommand();
            if (last != null) inputField.text = last;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            string next = console.GetNextCommand();
            if (next != null) inputField.text = next;
        }
    }

    class HelloCommand : ConsoleCommand
    {
        public override string Name => "hello";

        internal override string SuccessMessage => "Hello Success";

        internal override string ErrorMessage => "Hello Failure";

        internal override Delegate CallBack => new Action(() => Debug.Log("Hello"));

    }

    class ErrorCommand : ConsoleCommand
    {
        public override string Name => "error";

        internal override string SuccessMessage => "impossable";

        internal override string ErrorMessage => "yes, indeed";

        internal override Delegate CallBack => new Action(() => throw new NotImplementedException());

    }

    class PrintCommand : ConsoleCommand
    {
        public override string Name => "print";

        internal override string SuccessMessage => "print successs";

        internal override string ErrorMessage => "";

        internal override Delegate CallBack => new Action<string>(arg => Print(arg));

        protected void Print(string msg)
        {
            msg.Dump();
        }
    }

    class AddCommand : ConsoleCommand
    {
        public override string Name => "add";

        internal override string SuccessMessage => "";

        internal override string ErrorMessage => "add error";

        internal override Delegate CallBack => new Action<int, int>((a, b) => Add(a, b));

        protected void Add(int a, int b)
        {
            (a + b).Dump();
        }
    }
}
