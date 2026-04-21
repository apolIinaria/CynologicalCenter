using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CynologicalCenter.Models;

namespace CynologicalCenter.UI.Dialogs
{
    public partial class OwnerEditDialog : Window
    {
        private readonly int? _ownerId;
        public OwnerEditDialog()
        {
            InitializeComponent();
            _ownerId = null;
        }

        public OwnerEditDialog(int ownerId)
        {
            InitializeComponent();
            _ownerId = ownerId;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_ownerId.HasValue)
            {
                TxtTitle.Text = "Редагувати клієнта";
                var owner = await App.OwnerService.GetByIdAsync(_ownerId.Value);
                if (owner != null)
                {
                    TxtFullName.Text = owner.FullName;
                    TxtPhone.Text = owner.Phone ?? "";
                    TxtEmail.Text = owner.Email ?? "";
                }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Text = "";

            var owner = new Owner
            {
                OwnerId = _ownerId ?? 0,
                FullName = TxtFullName.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(TxtPhone.Text) ? null : TxtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim()
            };

            (bool success, string msg) result;

            if (_ownerId.HasValue)
                result = await App.OwnerService.UpdateAsync(owner);
            else
                result = await App.OwnerService.AddAsync(owner);

            if (result.success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                TxtError.Text = result.msg;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
