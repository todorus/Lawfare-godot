namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        
    }
    
    [Fact]
    public void This_test_should_fail_if_xunit_is_working()
    {
        Assert.True(false, "If you see this failure, the test project is wired correctly.");
    }
}