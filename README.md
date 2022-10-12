# Blazor BaiduMap 百度地图 组件

定位组件

示例:

https://blazor.app1.es/baidumap

使用方法:

1.nuget包

```BootstrapBlazor.BaiduMap```

2._Imports.razor 文件 或者页面添加 添加组件库引用

```@using BootstrapBlazor.Components```


3.razor页面
```
<BaiduMap OnError="@OnError" OnResult="@OnResult" />
```
```
@code{

    private string message;
    private BaiduItem baiduItem;

    private Task OnResult(BaiduItem geolocations)
    {
        baiduItem = geolocations;
        this.message = baiduItem.Status;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnError(string message)
    {
        this.message = message;
        StateHasChanged();
        return Task.CompletedTask;
    }

} 
```