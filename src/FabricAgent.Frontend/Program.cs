using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 서비스 등록
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// HttpClient Factory 등록 (Backend API 호출용)
builder.Services.AddHttpClient();

var app = builder.Build();

// 미들웨어 파이프라인 구성
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Blazor Server 엔드포인트 매핑
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
