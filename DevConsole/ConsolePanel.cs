using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UnityUtil.DevConsole
{
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
            console.AddCommand(new GeneralCommand(() => "holy shit".Dump(), "shit"));
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

        [OnInspector]
        public void DumpCommands()
        {
            console.DumpCommands();
        }

        [CommandType]
        class HelloCommand : ConsoleCommand
        {
            public override string Name => "hello";

            public override string SuccessMessage => "Hello Success";

            public override string ErrorMessage => "Hello Failure";

            public void OnExecute()
            {
                Debug.Log("Hello");
            }

        }

        [CommandType]
        class ErrorCommand : ConsoleCommand
        {
            public override string Name => "error";

            public override string SuccessMessage => "impossable";

            public override string ErrorMessage => "yes, indeed";

            public void OnExecute()
            {
                throw new NotImplementedException();
            }

        }

        [CommandType]
        class PrintCommand : ConsoleCommand
        {
            public override string Name => "print";

            public override string SuccessMessage => "print successs";

            public override string ErrorMessage => "";

            protected void OnExecute(string msg)
            {
                msg.Dump();
            }
        }

        [CommandType]
        class AddCommand : ConsoleCommand
        {
            public override string Name => "add";

            public override string SuccessMessage => "";

            public override string ErrorMessage => "add error";

            protected void OnExecute(int a, int b)
            {
                (a + b).Dump();
            }
        }
    }

}

