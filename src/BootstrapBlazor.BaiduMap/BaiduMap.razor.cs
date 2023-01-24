// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;
/// <summary>
/// 百度地图 BaiduMap 组件
/// </summary>
public partial class BaiduMap : IAsyncDisposable
{
    [Inject][NotNull] IJSRuntime? JS { get; set; }
    [Inject] IConfiguration? config { get; set; }

    /// <summary>
    /// 获得/设置 错误回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnError { get; set; }

    /// <summary>
    /// 获得/设置 BaiduKey<para></para>
    /// 为空则在 IConfiguration 服务获取 "BaiduKey" , 默认在 appsettings.json 文件配置
    /// </summary>
    [Parameter]
    public string? BaiduKey { get; set; }

    /// <summary>
    /// 获得/设置 style
    /// </summary>
    [Parameter]
    public string Style { get; set; } = "height:700px;width:100%;";

    /// <summary>
    /// 获得/设置 ID
    /// </summary>
    [Parameter]
    public string ID { get; set; } = "container";

    /// <summary>
    /// 获得/设置 定位结果回调方法
    /// </summary>
    [Parameter]
    public Func<BaiduItem, Task>? OnResult { get; set; }

    private IJSObjectReference? module;
    private DotNetObjectReference<BaiduMap>? InstanceGeo { get; set; }

    private string key = String.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            key = BaiduKey ?? (config != null ? config["BaiduKey"] : null) ?? "abcd";
            module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.BaiduMap/lib/baidu/baidumap.js");
            InstanceGeo = DotNetObjectReference.Create(this);
            while (!(await Init()))
            {
                await Task.Delay(500);
            }
            //await module!.InvokeVoidAsync("initMaps");
        }
    }

    public async Task<bool> Init() => await module!.InvokeAsync<bool>("addScript", new object?[] { key, ID, null, null, null });

    public async Task ResetMaps() => await module!.InvokeVoidAsync("resetMaps", ID);

    public async Task OnBtnClick(string btn) => await module!.InvokeVoidAsync(btn);

    /// <summary>
    /// 获取定位
    /// </summary>
    public virtual async Task GetLocation()
    {
        try
        {
            await module!.InvokeVoidAsync("geolocation", InstanceGeo);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }


    /// <summary>
    /// 定位完成回调方法
    /// </summary>
    /// <param name="geolocations"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task GetResult(BaiduItem geolocations)
    {
        try
        {
            if (OnResult != null) await OnResult.Invoke(geolocations);
        }
        catch (Exception e)
        {
            if (OnError != null) await OnError.Invoke(e.Message);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (module is not null)
        {
            //await module.InvokeVoidAsync("destroy", Options);
            await module.DisposeAsync();
        }
    }

}
