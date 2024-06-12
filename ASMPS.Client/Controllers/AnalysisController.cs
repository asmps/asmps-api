using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ASMPS.Client.Controllers;

public class AnalysisController : Controller
{
    private readonly HttpClient _httpClient;

    private void AddBearerToken()
    {
        var token = Request.Cookies["JWToken"];
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    public AnalysisController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<IActionResult> Student()
    {
        AddBearerToken();
        var response = await _httpClient.GetAsync("http://localhost:5190/api/Statistics/AttendanceRates");
        Console.WriteLine(response.IsSuccessStatusCode);
        Console.WriteLine(response.Headers);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var attendanceRates = JsonConvert.DeserializeObject<Dictionary<string, double>>(data);
            ViewBag.AttendanceRates = attendanceRates;
        }

        return View();
    }

    public async Task<IActionResult> AttendanceComparison()
    {
        AddBearerToken();
        var response = await _httpClient.GetAsync("http://localhost:5190/api/Statistics/AttendanceRateByGroup");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var groupAttendanceRates = JsonConvert.DeserializeObject<Dictionary<string, double>>(data);
            ViewBag.GroupAttendanceRates = groupAttendanceRates;
        }

        return View();
    }

    public async Task<IActionResult> ClassroomLoad()
    {
        AddBearerToken();
        var response = await _httpClient.GetAsync("http://localhost:5190/api/Statistics/AudienceOccupancy");
        Console.WriteLine(response.Headers);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();  
            var audienceOccupancy = JsonConvert.DeserializeObject<Dictionary<string, int>>(data);
            ViewBag.AudienceOccupancy = audienceOccupancy;
        }

        return View();
    }

    public async Task<IActionResult> TeacherEfficiency()
    {
        AddBearerToken();
        var response = await _httpClient.GetAsync("http://localhost:5190/api/Statistics/TeacherAttendanceRate");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var teacherAttendanceRates = JsonConvert.DeserializeObject<Dictionary<string, double>>(data);
            ViewBag.TeacherAttendanceRates = teacherAttendanceRates;
        }

        return View();
    }
}