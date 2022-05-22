using System;
using System.Collections.Generic;

namespace MiniShogiMobile.Conditions
{
    public class InputNameCondition
    {
        public string Title { get; }
        public string DefaultName { get; }
        public string OkButtonText { get; }
        public List<string> NameList { get; }
        public Func<string, string> NameErrorChecker { get; } = (x) => null;
        public Func<string, string> NameConfirmer { get; } = (x) => null;
        public InputNameCondition(string title, string defaultName, string okButtonName = "OK", List<string> nameList = null, Func<string, string> nameErrorChecker = null, Func<string, string> nameConfirmer = null)
        {
            Title = title;
            DefaultName = defaultName;
            OkButtonText = okButtonName;
            NameList = nameList;
            if(nameErrorChecker != null)
                NameErrorChecker = nameErrorChecker;
            if(nameConfirmer != null)
                NameConfirmer = nameConfirmer;
        }

    }
}
