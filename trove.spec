Name:     humble-trove
Version:  1.0
Release:  1%{?dist}
BuildArch: x86_64
Summary:  The cross platform Humble Trove Downloader
License:  MIT
URL:      https://github.com/td512/Humble-Trove-Console-Downloader   
%undefine _disable_source_fetch
Source0:  https://ltscdn.m6.nz/humble/1804/trove

%description
The cross platform Humble Trove Downloader. Simple, clean, and efficient.

%install
mkdir -p %{buildroot}/%{_bindir}
install -m 0755 %{_sourcedir}/trove %{buildroot}/%{_bindir}/trove

%files
%{_bindir}/trove

%changelog
* Sun Oct 27 2019 Theo Morra <theo@theom.nz> - 1.0-1
- Initial version of the package
