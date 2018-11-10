﻿#pragma once
#include "MainPage.g.h"

#include "Arc.h"
#include "CurrencyStringConverter.h"
#include "FluxStringConverter.h"
#include "InternetStatusStringConverter.h"
#include "MainViewModel.h"
#include "NetHelper.h"
#include "NetStateStringConverter.h"
#include "TimeSpanStringConverter.h"
#include <memory>

namespace winrt::TsinghuaNetUWP::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
    public:
        MainPage();

        void OpenSettings(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);
        Windows::Foundation::IAsyncAction Login(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);
        Windows::Foundation::IAsyncAction Logout(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);
        Windows::Foundation::IAsyncAction Refresh(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);

        Windows::Foundation::IAsyncAction ShowChangeUser(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);

        void StateChanged(Windows::Foundation::IInspectable const& sender, NetState const& e);
        Windows::Foundation::IAsyncAction Auth4Checked(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);
        Windows::Foundation::IAsyncAction Auth6Checked(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);
        Windows::Foundation::IAsyncAction NetChecked(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);

        void RefreshStatus(Windows::Foundation::IInspectable const& sender, Windows::UI::Xaml::RoutedEventArgs const& e);

    private:
        Windows::Foundation::IAsyncAction LoginImpl();
        Windows::Foundation::IAsyncAction LogoutImpl();
        Windows::Foundation::IAsyncAction RefreshImpl();
        Windows::Foundation::IAsyncAction RefreshImpl(IConnect const& helper);
        std::unique_ptr<IConnect> GetHelper();

        void RefreshStatusImpl();
    };
} // namespace winrt::TsinghuaNetUWP::implementation

namespace winrt::TsinghuaNetUWP::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
} // namespace winrt::TsinghuaNetUWP::factory_implementation
