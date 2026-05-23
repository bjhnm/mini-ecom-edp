using mvvm_edp.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace mvvm_edp.ViewModels;

public interface ICreateTab : ITabViewModel
{
    void OnTabActivated();
}
