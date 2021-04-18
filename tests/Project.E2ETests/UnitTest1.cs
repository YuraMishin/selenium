using System;
using System.Runtime.InteropServices;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Project.E2ETests
{
  public class UnitTest1 : IDisposable
  {
    #region Bootstrap

    public readonly IWebDriver Driver;
    public UnitTest1()
    {
      // Set up (called once per test)
      Driver = CreateWebDriver();
    }

    public void Dispose()
    {
      // Tear down (called once per test)
      Driver.Quit();
    }

    private static IWebDriver CreateWebDriver()
    {
      string driverDirectory = System.IO.Path.GetDirectoryName(typeof(UnitTest1).Assembly.Location) ?? ".";
      bool isDebuggerAttached = System.Diagnostics.Debugger.IsAttached;

      return CreateChromeDriver(driverDirectory, isDebuggerAttached);
    }

    private static IWebDriver CreateChromeDriver(
      string driverDirectory,
      bool isDebuggerAttached)
    {
      var options = new ChromeOptions();

      if (!isDebuggerAttached)
      {
        //options.AddArgument("--headless");
      }

      // HACK Workaround for "(unknown error: DevToolsActivePort file doesn't exist)"
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
      {
        options.AddArgument("--no-sandbox");
      }

      return new ChromeDriver(driverDirectory, options);
    }

    private void HighlightElementUsingJavaScript(By locationStrategy, int duration = 2)
    {
      var element = Driver.FindElement(locationStrategy);
      var originalStyle = element.GetAttribute("style");
      IJavaScriptExecutor JavaScriptExecutor = Driver as IJavaScriptExecutor;
      JavaScriptExecutor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])",
        element,
        "style",
        "border: 7px solid yellow; border-style: dashed;");

      if (duration <= 0) return;
      Thread.Sleep(TimeSpan.FromSeconds(duration));
      JavaScriptExecutor.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2])",
        element,
        "style",
        originalStyle);
    }

    #endregion

    [Fact]
    public void Method_Returns_When()
    {
      // Arrange

      // Act
      var url = "https://www.ultimateqa.com/simple-html-elements-for-automation/";
      Driver.Navigate().GoToUrl(url);
      HighlightElementUsingJavaScript(By.ClassName("buttonClass"));
      HighlightElementUsingJavaScript(By.Id("idExample"));
      HighlightElementUsingJavaScript(By.LinkText("Click me using this link text!"));
      HighlightElementUsingJavaScript(By.Name("button1"));
      HighlightElementUsingJavaScript(By.PartialLinkText("link text!"));
      HighlightElementUsingJavaScript(By.TagName("div"));
      HighlightElementUsingJavaScript(By.CssSelector("#idExample"));
      HighlightElementUsingJavaScript(By.CssSelector(".buttonClass"));
      HighlightElementUsingJavaScript(By.XPath("//*[@id='idExample']"));
      HighlightElementUsingJavaScript(By.XPath("//*[@class='buttonClass']"));

      // Assert
      Assert.True(true);
    }

    [Fact]
    public void Highlight_The_Link()
    {
      // Arrange
      var expected = "Click this link";

      // Act
      var url = "https://www.ultimateqa.com/simple-html-elements-for-automation/";
      Driver.Navigate().GoToUrl(url);
      var className = "et_pb_blurb_description";
      var links = Driver.FindElements(By.ClassName(className));
      var linkOption1 = links[3];
      HighlightElementUsingJavaScript(By.Id("simpleElementsLink"));
      HighlightElementUsingJavaScript(By.LinkText("Click this link"));
      HighlightElementUsingJavaScript(By.Name("clickableLink"));
      HighlightElementUsingJavaScript(By.PartialLinkText("Click this lin"));
      HighlightElementUsingJavaScript(By.CssSelector("#simpleElementsLink"));
      HighlightElementUsingJavaScript(By.XPath("//*[@id='simpleElementsLink']"));

      // Assert
      Assert.Equal(expected, linkOption1.Text);

    }

    [Fact]
    public void XPathLocation()
    {
      var url = "https://www.ultimateqa.com/simple-html-elements-for-automation/";
      Driver.Navigate().GoToUrl(url);

      //click any radio button
      var radioBtn = Driver.FindElement(By.XPath("//*[@type='radio'][@value='male']"));
      radioBtn.Click();

      //select one checkbox
      var checkBox = Driver.FindElement(By.XPath("//*[@value='Bike']"));
      checkBox.Click();

      //select Audi from the dropdown
      Driver.FindElement(By.TagName("select")).Click();
      Driver.FindElement(By.XPath("//*[@value='audi']")).Click();

      //open Tab2 and assert that it is opened.
      //Hint, use .Text property when you find the element
      Driver.FindElement(By.XPath("//li[@class='et_pb_tab_1']")).Click();
      //WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
      //wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@class='et_pb_tab et_pb_tab_1 clearfix et-pb-active-slide']")));
      Assert.Equal("Tab 1 content",
        Driver.FindElement(By.XPath("//*[@class='et_pb_tab et_pb_tab_1 clearfix et-pb-active-slide']")).Text);
      //in the HTML Table with id, highlight one of the salary cells
      HighlightElementUsingJavaScript(By.XPath("//td[contains(text(),'$150,000+')]"));

      //Highlight the center section called "Highlight me", but you can only
      //highlight the highest level div for that element. The top parent div.
      //Hint, this is the class - 
      //et_pb_column et_pb_column_1_3  et_pb_column_10 et_pb_css_mix_blend_mode_passthrough
      HighlightElementUsingJavaScript(By.XPath("//*[@class='et_pb_column et_pb_column_1_3 et_pb_column_10  et_pb_css_mix_blend_mode_passthrough']"));
    }
  }
}
