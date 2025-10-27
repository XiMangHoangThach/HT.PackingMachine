using Blazored.LocalStorage;
using HoangThach.AccountShared.Models.Entities;
using HoangThach.AccountShared.Services;
using HT.PackingMachine.Data;
using HT.PackingMachine.Data.Entities;
using HT.PackingMachine.Data.Enums;
using HT.PackingMachine.Data.Models;
using HT.PackingMachine.Data.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Security.Claims;

namespace HT.PackingMachine.Components.Pages
{
    public partial class SoLoVoBao
    {
        [Inject] XhsContext? XhsContext { get; set; }
        [Inject] XhsSpServices? XhsSpServices { get; set; }
        [Inject] ISnackbar? Snackbar { get; set; }
        [Inject] NavigationManager? NavigationManager { get; set; }
        private SoLoVoBaoModel Input { get; set; } = new();

        private List<SoLoVoBaoModel> BatchNumLists { get; set; } = new();
        private List<Category> Categories { get; set; } = new();
        private Category Category { get; set; } = new();

        [CascadingParameter]
        private Task<AuthenticationState>? authenticationState { get; set; }

        private bool ShowProgress;
        private string searchString = "";

        public string ShowEdit = "d-none";
        public string ShowDelete = "d-none";
        public string ShowCreate = "d-flex";
        public bool DisabledSoLo = false;

        public string? Gid = "";
        public int UserId = 0;
        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
        }

        // đảm bảo đã hoàn thành render
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

            //if (UserId == 0) return;

            try
            {
                //Kiểm tra số lô đã tồn tại chưa
                var checkBatchNumList = await XhsContext.BatchNum_Lists.Include(s => s.Category)
                   .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
                   .Where(x => x.BatchNum == Input.BatchNum)
                   .Where(x => x.Category.PackingType == (byte)HTPackingTypeEnum.BAO)
                   .Where(x => x.Category.Id == Input.Category_Id).ToListAsync();

                if (checkBatchNumList.Any())
                {
                    Snackbar!.Add("Số lô đã tồn tại, xin nhập số lô khác!", Severity.Error);
                }
                else
                {
                    //Nếu chưa thì thêm mới
                    BatchNum_List batchNum_List = new()
                    {
                        BatchNum = Input.BatchNum,
                        Category_Id = Input.Category_Id,
                        Qty = Input.Qty,
                        CreatedBy = UserId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                        Status = (byte)HTStatusCodeEnum.Stable
                    };

                    try
                    {
                        XhsSpServices?.P_BatchNumList_Create(batchNum_List);
                    }
                    catch (Exception ex)
                    {
                        await Reload(ex.Message, Severity.Success);

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

            // if (UserId == 0) return;

            try
            {
                //Kiểm tra Số lô đã được đăng ký cho máy đóng chưa
                var checkBatchNumRegisterCancel = await XhsContext!.BatchNum_Registers
                              .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.BatchNum_List_Id == Input.Id).ToListAsync();

                //Tổng số lượng vỏ của số lô đã đăng ký cho máy đóng
                var totalRegister = await XhsContext.BatchNum_Registers.Where(b => b.BatchNum_List_Id == Input.Id).SumAsync(a => a.Qty);

                //Lấy số lô theo id
                var currentBatchNum = await XhsContext.BatchNum_Lists.FindAsync(Input.Id);

                //Không tồn tại đăng ký cho máy đóng, hoặc có đăng ký nhưng số lượng vỏ sử dụng bằng số lượng số lô hiện tại
                if (!checkBatchNumRegisterCancel.Any() || (checkBatchNumRegisterCancel.Any() && totalRegister == currentBatchNum!.Qty))
                {
                    //Thì được xóa
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
                        await Reload(ex.Message, Severity.Success);

                    }

                    await Reload("Xóa số lô thành công!", Severity.Success);
                }
                else
                {
                    Snackbar?.Add("Không thể lưu thay đổi. Số lô đang được sử dụng!", Severity.Error);
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
                //Kiểm tra đã có đăng ký số lô cho máy đóng hay chưa
                var checkBatchNumRegisterUpdate = await XhsContext!.BatchNum_Registers
                     .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable && a.BatchNum_List_Id == Input.Id).ToListAsync();

                //Nếu không được phép update
                if (!checkBatchNumRegisterUpdate.Any())
                {
                    BatchNum_List batchNum_List = new()
                    {
                        Id = Input.Id,
                        Category_Id = Input.Category_Id,
                        Qty = Input.Qty,
                        ModifiedBy = UserId,
                        ModifiedDate = DateTime.Now,
                    };

                    try
                    {
                        XhsSpServices!.P_BatchNumList_Update(batchNum_List);
                    }
                    catch (Exception ex)
                    {
                        await Reload(ex.Message, Severity.Success);

                    }

                    await Reload("Cập nhật số lô thành công!", Severity.Success);
                }
                else
                {
                    Snackbar?.Add("Không thể lưu thay đổi. Số lô đang được sử dụng!", Severity.Error);
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
                    Category_Id = data.Category_Id,
                    Qty = data.Qty
                };
            }
        }
        private bool FilterFunc(SoLoVoBaoModel element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.BatchNum.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
        private async Task<List<SoLoVoBaoModel>> GetBatchNumListAsync()
        {
            BatchNumLists = new();
            var data = await XhsContext.BatchNum_Lists
            .Include(p => p.Category)
            .Where(a => a.Status == (byte)HTStatusCodeEnum.Stable)
            .Where(p => p.Category.PackingType == (int)HTPackingTypeEnum.BAO)
            .OrderByDescending(c => c.Id)
            .AsNoTracking()
            .ToListAsync();

            foreach (var item in data)
            {
                SoLoVoBaoModel batchNum = new() { Id = item.Id, BatchNum = item.BatchNum, Category = item.Category, Qty = item.Qty, CreatedBy = item.CreatedBy, CreatedDate = item.CreatedDate };

                BatchNumLists.Add(batchNum);
            }

            return BatchNumLists;
        }
        private async Task<List<Category>> GetCategoriesAsync()
        {
            Categories = new();
            Categories = await XhsContext!.Category.Where(x => x.PackingType == (int)HTPackingTypeEnum.BAO)
                .Where(x => x.Status == (byte)HTStatusCodeEnum.Stable).AsNoTracking().ToListAsync();

            return Categories;
        }
        private void RowClickEvent(TableRowClickEventArgs<SoLoVoBaoModel> tableRowClickEventArgs)
        {
            Input = new();
        }
    }
}
