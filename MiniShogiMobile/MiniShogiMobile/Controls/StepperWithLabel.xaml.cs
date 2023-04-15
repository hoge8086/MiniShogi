using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MiniShogiMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StepperWithLabel : ContentView
    {
        public StepperWithLabel()
        {
            InitializeComponent();
            plusButton.Clicked += (s, e) =>
            {
                if (Value < MaximumValue)
                    Value++;
            };
            minusButton.Clicked += (s, e) =>
            {
                if (Value > MinimumValue)
                    Value--;
            };
        }

        public static readonly BindableProperty ValueProperty =
          BindableProperty.Create(
             nameof(Value),
              returnType: typeof(int),
              declaringType: typeof(StepperWithLabel),
              defaultValue: 0,
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: OnValueChanged);

        static void OnValueChanged (BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as StepperWithLabel;
            if (view == null || newValue == oldValue)
                return;
            view.UpdateIsEnable();
        }

        private void UpdateIsEnable()
        {
            if (Value >= MaximumValue || !IsEnabled)
                plusButton.IsEnabled = false;
            else
                plusButton.IsEnabled = true;

            if (Value <= MinimumValue || !IsEnabled)
                minusButton.IsEnabled = false;
            else
                minusButton.IsEnabled = true;
        }
        public static readonly BindableProperty UnitLabelProperty =
            BindableProperty.Create(nameof(UnitLabel), typeof(string), typeof(StepperWithLabel), defaultValue: string.Empty);

        public static readonly BindableProperty MinimumValueProperty =
            BindableProperty.Create(nameof(MinimumValue), typeof(int), typeof(StepperWithLabel), defaultValue: 0);

        public static readonly BindableProperty MaximumValueProperty =
            BindableProperty.Create(nameof(MaximumValue), typeof(int), typeof(StepperWithLabel), defaultValue: 10);

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public string UnitLabel
        {
            get { return (string)GetValue(UnitLabelProperty); }
            set { SetValue(UnitLabelProperty, value); }
        }

        public int MinimumValue
        {
            get { return (int)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        public int MaximumValue
        {
            get { return (int)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }
        public static new readonly BindableProperty IsEnabledProperty = BindableProperty.Create(
                                                                            nameof(IsEnabled),
                                                                            typeof(bool),
                                                                            typeof(StepperWithLabel),
                                                                            true,
                                                                            propertyChanged: OnIsEnabledChanged);
 
        static void OnIsEnabledChanged (BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as StepperWithLabel;
            if (view == null || newValue == oldValue)
                return;
            view.UpdateIsEnable();
        }
        public new bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }
    }
}