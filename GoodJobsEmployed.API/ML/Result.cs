namespace GoodJobsEmployed.API.ML;

public class Result
{
    public bool Correct { get; set; }
    public string ErrorMessage { get; set; }
    public object Object { get; set; }
    public List<object> Objects { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public Exception Ex { get; set; }
}
