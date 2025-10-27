using HT.PackingMachine.Data;
using HT.PackingMachine.Data.Entities;
using HT.PackingMachine.Data.Enums;
using HT.PackingMachine.Data.Models;
using HT.PackingMachine.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HT.PackingMachine.Components.Pages
{
    public partial class SoLoChatLuong
    {
        [Inject] XhsContext? XhsContext { get; set; }
        [Inject] XhsSpServices? XhsSpServices { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        [Inject] NavigationManager? NavigationManager { get; set; }
        private SoLoChatLuongModel Input { get; set; } = new();

        private List<SoLoChatLuongModel> BatchNumLists { get; set; } = new();
        private List<Category> Categories { get; set; } = new();


        private bool ShowProgress;
        private string searchString = "";

        public string ShowEdit = "d-none";
        public string ShowDelete = "d-none";
        public string ShowCreate = "d-flex";
        public bool DisabledSoLo = false;

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        public int UserId = 0;
        public string? Gid = "";
        protected override async Task OnInitializedAsync()
        {


            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await HandleToken();

                await GetCategoriesAsync();
                await GetBatchNumListAsync();
            
                StateHasChanged();
            }
        }
        private async Task HandleToken()
        {
            var token = await AuthService.CheckTokenAndRefresh(null);

            if (authenticationState is not null)
            {
                var authState = await authenticationState;
                var user = authState?.User;

                if (user?.Identity is not null && user.Identity.IsAuthenticated && !string.IsNullOrEmpty(token))
                {
                    UserId = int.Parse(user.Claims.First().Value);
                    Gid = user.FindFirst(c => c.Type == "Gid")?.Value;
                }
                else
                {
                    NavigationManager.NavigateTo("Login");
                }
            }
            else
            {
                NavigationManager.NavigateTo("Login");
            }
        }
        private async Task Reload(string message, Severity severity)
        {
            await Task.Delay(300);
            ShowCreate = "d-flex";
            ShowEdit = "d-none";
            ShowDelete = "d-none";
            DisabledSoLo = false;
            Snackbar!.Add(message, severity);

            await GetCategoriesAsync();
            await GetBatchNumListAsync();
            Input = new();
            await InvokeAsync(StateHasChanged);
        }
        private async void OnValidSubmit(EditContext context)
        {
            await Create();
            StateHasChanged();
        }
        private async Task Create()
        {
            ShowProgress = true;
            await Task.Delay(300);
            if (string.IsNullOrEmpty(Input.BatchNum))
            {
                await Reload("Chưa nhập số lô", Severity.Error);
                return;
            }

            if (string.IsNullOrEmpty(Input.QualityNum))
            {
                await Reload("Chưa nhập số chất lượng", Severity.Error);
                return;
            }

            if (Input.CategoryId == 0)
            {
                await Reload("Chưa chọn chủng loại", Severity.Error);
                return;
            }

            try
            {
                //Kiểm tra Số lô chất lượng đã được đăng ký chưa
                var checkBatchNumList = await XhsContext!.BatchNum_Lists.Include(s => s.Category)
                   .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                   .Where(x => x.BatchNum == Input.BatchNum)
                   .Where(x => x.QualityNum == Input.QualityNum)
                   .Where(c => c.Category.PackingType == (byte)HTPackingTypeEnum.ROI).ToListAsync();

                if (checkBatchNumList.Any())
                {
                    Snackbar!.Add("Số lô đã tồn tại, xin nhập số lô khác!", Severity.Error);
                }
                else
                {
                    //Nếu chưa thì được đăng ký mới
                    SoLoChatLuongModel  batchNum_List = new()
                    {
                        BatchNum = Input.BatchNum,
                        CategoryId = Input.CategoryId,
                        Qty = Input.Qty,
                        QualityDate = Input.QualityDate,
                        QualityNum = Input.QualityNum,
                        CreatedBy = UserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                        Status = (byte)HTStatusCodeEnum.Stable
                    };

                    try
                    {
                        XhsSpServices?.P_BatchNumQuality_Create(batchNum_List);
                    }
                    catch (Exception ex)
                    {
                        await Reload(ex.Message, Severity.Error);

                    }

                    await Reload("Tạo số lô thành công!", Severity.Success);
                }

            }
            catch (Exception ex)
            {
                await Reload(ex.Message, Severity.Error);

            }
            ShowProgress = false;
        }
        private async Task Delete()
        {
            ShowProgress = true;
            await Task.Delay(300);
            //if (UserId == 0) return;
            try
            {
                //Kiểm tra Số lô có tồn tại hay ko
                var updateBatchNumListCancel = await XhsContext!.BatchNum_Lists
                .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.Id).ToListAsync();

                if (updateBatchNumListCancel.Any())
                {
                    //Nếu tồn tại được xóa
                    BatchNum_List batchNum_List = new()
                    {
                        Id = Input.Id,
                        Status = (byte)HTStatusCodeEnum.Delete,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                    };

                    try
                    {
                        XhsSpServices!.P_BatchNumList_Cancel(batchNum_List);
                    }
                    catch (Exception ex)
                    {
                        await Reload(ex.Message, Severity.Error);

                    }

                    await Reload("Xóa số lô thành công!", Severity.Success);
                }


            }
            catch (Exception ex)
            {
                Snackbar?.Add(ex.Message, Severity.Error);

            }
            ShowProgress = false;
            StateHasChanged();
        }
        private async Task Update()
        {
            ShowProgress = true;
            await Task.Delay(300);
            //if (UserId == 0) return;
            try
            {
                //Kiểm tra số lô có tồn tại hay ko
                var updateBatchNumListUpdate = await XhsContext!.BatchNum_Lists
                    .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.Id == Input.Id).ToListAsync();

                if (updateBatchNumListUpdate.Any())
                {
                    //nếu tồn tại được cập nhật
                    SoLoChatLuongModel batchNum_List = new()
                    {
                        Id = Input.Id,
                        CategoryId = Input.CategoryId,
                        Qty = Input.Qty,
                        QualityNum = Input.QualityNum,
                        QualityDate = Input.QualityDate,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                    };

                    try
                    {
                        XhsSpServices!.P_BatchNumQuality_Update(batchNum_List);
                    }
                    catch (Exception ex)
                    {
                        await Reload(ex.Message, Severity.Error);

                    }

                    await Reload("Cập nhật số lô thành công!", Severity.Success);
                }
            }
            catch (Exception ex)
            {
                Snackbar?.Add(ex.Message, Severity.Error);

            }
            ShowProgress = false;
            StateHasChanged();
        }
        private async Task Back()
        {
            await Reload("", Severity.Normal);
        }
        private async Task SelectBatchNumList(int id)
        {
            Input = new();
            ShowEdit = "d-flex";
            ShowDelete = "d-flex";
            ShowCreate = "d-none";
            DisabledSoLo = true;
            var data = await XhsContext!.BatchNum_Lists.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                Input = new()
                {
                    Id = data.Id,
                    BatchNum = data.BatchNum,
                    QualityNum = data.QualityNum,
                    QualityDate = data.QualityDate,
                    CategoryId = data.Category_Id,
                    Qty = data.Qty
                };
            }
        }
        private bool FilterFunc(SoLoChatLuongModel element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.BatchNum.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private async Task<List<SoLoChatLuongModel>> GetBatchNumListAsync()
        {
            BatchNumLists = new();
            var data = await XhsContext!.BatchNum_Lists.Include(p => p.Category)
                .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                .Where(p => p.Category.PackingType == (int)HTPackingTypeEnum.ROI)
                .OrderByDescending(c => c.Id).AsNoTracking().ToListAsync();
            foreach (var item in data)
            {
                SoLoChatLuongModel batchNum = new()
                {
                    Id = item.Id,
                    BatchNum = item.BatchNum,
                    Category = item.Category,
                    Qty = item.Qty,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    QualityDate = item.QualityDate,
                    QualityNum = item.QualityNum
                };

                BatchNumLists.Add(batchNum);
            }
            return BatchNumLists;
        }
        private async Task<List<Category>> GetCategoriesAsync()
        {
            Categories = new();
            Categories = await XhsContext!.Category.Where(x => x.PackingType == (int)HTPackingTypeEnum.ROI)
                .Where(x => x.Status == (byte)HTStatusCodeEnum.Stable).AsNoTracking().ToListAsync();

            return Categories;
        }
        private void RowClickEvent(TableRowClickEventArgs<SoLoChatLuongModel> tableRowClickEventArgs)
        {
            Input = new();
        }
    }
}
