using GoodTimeStudio.MyPhone.Pages;
using Xunit;

namespace MyPhone.UnitTest.ViewModels
{
    public class TestCallPageViewModel
    {
        private CallPageViewModel ViewModel;

        public TestCallPageViewModel(CallPageViewModel callPageViewModel)
        {
            ViewModel = callPageViewModel;
        }

        private void InputOneNine()
        {
            for (var i = 1; i < 10; i++)
            {
                ViewModel.PressDigit(i.ToString());
            }
        }

        [Fact]
        public void TestPressDigit()
        {
            InputOneNine();

            Assert.Equal("123456789", ViewModel.PhoneNumber);
            Assert.Equal(0, ViewModel.SelectionLength);
            Assert.Equal(9, ViewModel.SelectionStart);
        }

        [Fact]
        public void TestPressDigitWhenSelected()
        {
            ViewModel.PhoneNumber = "123456789";
            ViewModel.SelectionStart = 3;
            ViewModel.SelectionLength = 3;
            for (var i = 0; i < 3; i++)
            {
                ViewModel.PressDigit("6");
            }

            Assert.Equal("123666789", ViewModel.PhoneNumber);
            Assert.Equal(6, ViewModel.SelectionStart);
            Assert.Equal(0, ViewModel.SelectionLength);
        }

        [Fact]
        public void TestBackSpaceWithExistingPhoneNumber()
        {
            InputOneNine();

            for (var i = 1; i < 10; i++)
            {
                ViewModel.PressBackSpace();
            }

            Assert.Equal("", ViewModel.PhoneNumber);
            Assert.Equal(0, ViewModel.SelectionLength);
            Assert.Equal(0, ViewModel.SelectionStart);
        }

        [Fact]
        public void TestBackSpaceWithEmptyPhoneNumber()
        {
            for (var i = 0; i < 5; i++)
            {
                ViewModel.PressBackSpace();
            }

            Assert.True(string.IsNullOrEmpty(ViewModel.PhoneNumber));
            Assert.Equal(0, ViewModel.SelectionLength);
            Assert.Equal(0, ViewModel.SelectionStart);
        }

        [Fact]
        public void TestBackSpaceWhenSelected()
        {
            ViewModel.PhoneNumber = "123456789";
            ViewModel.SelectionStart = 3;
            ViewModel.SelectionLength = 3;
            ViewModel.PressBackSpace();

            Assert.Equal("123789", ViewModel.PhoneNumber);
            Assert.Equal(3, ViewModel.SelectionStart);
            Assert.Equal(0, ViewModel.SelectionLength);
        }
    }
}