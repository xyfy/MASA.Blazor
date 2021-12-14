﻿using System.Text;
using System.Threading.Tasks;
using BlazorComponent;
using Microsoft.AspNetCore.Components;

namespace MASA.Blazor
{
    //Todo：Crossover mode to be perfected
    public partial class MImage : MResponsive, IImage, IThemeable
    {
        [Parameter] 
        public bool Contain { get; set; }

        [Parameter] 
        public string Src { get; set; }

        [Parameter] 
        public string LazySrc { get; set; }

        [Parameter] 
        public string Gradient { get; set; }

        [Parameter] 
        public RenderFragment PlaceholderContent { get; set; }

        [Parameter] 
        public bool Dark { get; set; }

        [Parameter] 
        public bool Light { get; set; }

        [CascadingParameter] 
        public IThemeable Themeable { get; set; }

        [Parameter] 
        public string Position { get; set; } = "center center";

        public bool IsDark
        {
            get
            {
                if (Dark)
                {
                    return true;
                }

                if (Light)
                {
                    return false;
                }

                return Themeable != null && Themeable.IsDark;
            }
        }

        private string CurrentSrc { get; set; }

        public bool IsLoading { get; set; } = true;

        private bool IsError { get; set; } = false;

        private StringNumber CalculatedLazySrcAspectRatio { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
            }

            if (!string.IsNullOrEmpty(LazySrc) && IsLoading && Dimensions == null)
            {
                var dimensions = await JsInvokeAsync<Dimensions>(JsInteropConstants.GetImageDimensions, LazySrc);
                await PollForSize(dimensions);
                if (!dimensions.HasError && AspectRatio == null)
                {
                    AspectRatio = dimensions.Width / dimensions.Height;
                    CalculatedLazySrcAspectRatio = AspectRatio;
                }

                await InvokeStateHasChangedAsync();
            }

            if (!string.IsNullOrEmpty(Src) && IsLoading && !IsError)
            {
                var dimensions = await JsInvokeAsync<Dimensions>(JsInteropConstants.GetImageDimensions, Src);
                await PollForSize(dimensions);
                IsError = dimensions.HasError;
                if (!dimensions.HasError)
                {
                    IsLoading = false;
                    if (AspectRatio == null || CalculatedLazySrcAspectRatio != null)
                    {
                        AspectRatio = dimensions.Width / dimensions.Height;
                    }
                }

                await InvokeStateHasChangedAsync();
            }
        }

        protected override void SetComponentClass()
        {
            base.SetComponentClass();
            CssProvider
                .Merge(cssBuilder =>
                {
                    cssBuilder
                        .Add("m-image")
                        .AddTheme(IsDark);
                })
                .Apply("image", cssBuilder =>
                {
                    cssBuilder.Add("m-image__image")
                        .AddIf("m-image__image--preload", () => IsLoading)
                        .AddIf("m-image__image--contain", () => Contain)
                        .AddIf("m-image__image--cover", () => !Contain);
                }, styleBuilder =>
                {
                    string backgroud = GetBackgroudImage();
                    styleBuilder
                        .AddIf(backgroud, () => !string.IsNullOrEmpty(backgroud))
                        .AddIf($"background-position: {Position}", () => !string.IsNullOrEmpty(Position));
                })
                .Apply("placeholder", cssBuilder => { cssBuilder.Add("m-image__placeholder"); });

            AbstractProvider
                .Apply(typeof(BImageContent<>), typeof(BImageContent<MImage>))
                .Apply(typeof(BPlaceholderSlot<>), typeof(BPlaceholderSlot<MImage>))
                .Merge(typeof(BResponsiveBody<>), typeof(BImageBody<MImage>))
                .Apply<BResponsive, MResponsive>(attrs =>
                {
                    attrs[nameof(Dimensions)] = Dimensions;
                    attrs[nameof(AspectRatio)] = AspectRatio;
                    attrs[nameof(ContentClass)] = ContentClass;
                    attrs[nameof(Height)] = Height;
                    attrs[nameof(MinHeight)] = MinHeight;
                    attrs[nameof(MaxHeight)] = MaxHeight;
                    attrs[nameof(MinWidth)] = MinWidth;
                    attrs[nameof(MaxWidth)] = MaxWidth;
                });
        }

        private string GetBackgroudImage()
        {
            if (string.IsNullOrEmpty(Src) && string.IsNullOrEmpty(LazySrc) && string.IsNullOrEmpty(Gradient))
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("background-image:");
            if (!string.IsNullOrEmpty(Gradient))
            {
                stringBuilder.Append($"linear-gradient({Gradient}),");
            }

            var url = IsLoading || IsError ? LazySrc : CurrentSrc;
            stringBuilder.Append($"url({url})");
            return stringBuilder.ToString();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            CurrentSrc = Src;
        }

        private async Task PollForSize(Dimensions dimensions, int? timeOut = 100)
        {
            if (IsLoading && !dimensions.HasError && timeOut != null)
            {
                await Task.Delay(timeOut.Value);
            }

            Dimensions = dimensions;
        }
    }
}