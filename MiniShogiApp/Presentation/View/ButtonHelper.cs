using System.Windows;
using System.Windows.Controls.Primitives;


//CommandParameterプロパティがCommandプロパティの前にバインディングされて、
//CommandParameterプロパティがnullのまま、Command.CanExecute()呼ばれてしまい、
//コマンドの実行可否が正しく判定されない問題を解決する

//参考:
//https://code-examples.net/ja/q/51fe9
//http://toritonics.blogspot.com/2015/11/title.html


namespace MiniShogiApp.Presentation.View
{
    public static class ButtonHelper
    {
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(ButtonHelper),
            new PropertyMetadata(CommandParameter_Changed));

        private static void CommandParameter_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as ButtonBase;
            if (target == null)
                return;

            target.CommandParameter = e.NewValue;
            var temp = target.Command;
            // Have to set it to null first or CanExecute won't be called.
            target.Command = null;
            target.Command = temp;
        }

        public static object GetCommandParameter(ButtonBase target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(ButtonBase target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
    }
}
