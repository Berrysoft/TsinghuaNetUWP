﻿#include "pch.h"

#include "MainViewModel.h"

using namespace std;
using namespace winrt;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace TsinghuaNetHelper;

namespace winrt::TsinghuaNetUWP::implementation
{
    MainViewModel::MainViewModel()
    {
        m_NetUsers = single_threaded_observable_vector<IInspectable>();
    }

    void MainViewModel::OnAutoLoginPropertyChanged(DependencyObject const& d, DependencyPropertyChangedEventArgs const& e)
    {
        if (TsinghuaNetUWP::MainViewModel model{ d.try_as<TsinghuaNetUWP::MainViewModel>() })
        {
            MainViewModel* pm(get_self<MainViewModel>(model));
            pm->m_AutoLoginChangedEvent(model, unbox_value<bool>(e.NewValue()));
        }
    }

    void MainViewModel::OnBackgroundAutoLoginPropertyChanged(DependencyObject const& d, DependencyPropertyChangedEventArgs const& e)
    {
        if (TsinghuaNetUWP::MainViewModel model{ d.try_as<TsinghuaNetUWP::MainViewModel>() })
        {
            MainViewModel* pm(get_self<MainViewModel>(model));
            pm->m_BackgroundAutoLoginChangedEvent(model, unbox_value<bool>(e.NewValue()));
        }
    }

    void MainViewModel::OnBackgroundLiveTilePropertyChanged(DependencyObject const& d, DependencyPropertyChangedEventArgs const& e)
    {
        if (TsinghuaNetUWP::MainViewModel model{ d.try_as<TsinghuaNetUWP::MainViewModel>() })
        {
            MainViewModel* pm(get_self<MainViewModel>(model));
            pm->m_BackgroundLiveTileChangedEvent(model, unbox_value<bool>(e.NewValue()));
        }
    }

    DEPENDENCY_PROPERTY_INIT(OnlineUser, hstring, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(hstring()))
    DEPENDENCY_PROPERTY_INIT(Flux, uint64_t, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value<uint64_t>(0))
    DEPENDENCY_PROPERTY_INIT(OnlineTime, TimeSpan, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(TimeSpan()))
    DEPENDENCY_PROPERTY_INIT(Balance, double, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(0.0))

    DEPENDENCY_PROPERTY_INIT(FluxPercent, double, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(0.0))
    DEPENDENCY_PROPERTY_INIT(FreePercent, double, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(100.0))

    DEPENDENCY_PROPERTY_INIT(Username, hstring, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(hstring()))
    DEPENDENCY_PROPERTY_INIT(Password, hstring, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(hstring()))

    DEPENDENCY_PROPERTY_INIT(AutoLogin, bool, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(true), &MainViewModel::OnAutoLoginPropertyChanged)
    DEPENDENCY_PROPERTY_INIT(BackgroundAutoLogin, bool, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(true), &MainViewModel::OnBackgroundAutoLoginPropertyChanged)
    DEPENDENCY_PROPERTY_INIT(BackgroundLiveTile, bool, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(true), &MainViewModel::OnBackgroundLiveTilePropertyChanged)

    DEPENDENCY_PROPERTY_INIT(NetStatus, InternetStatus, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(InternetStatus::Unknown))
    DEPENDENCY_PROPERTY_INIT(Ssid, hstring, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(hstring()))
    DEPENDENCY_PROPERTY_INIT(SuggestState, NetState, MainViewModel, TsinghuaNetUWP::MainViewModel, box_value(NetState::Unknown))
} // namespace winrt::TsinghuaNetUWP::implementation
