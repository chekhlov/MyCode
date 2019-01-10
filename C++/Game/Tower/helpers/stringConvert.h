#pragma once
#include <string>


std::string narrow(const std::wstring& wide);
std::string narrow(const std::wstring& wide, const std::locale& loc);
std::wstring widen(const std::string& narrow);
std::wstring widen(const std::string& narrow, const std::locale& loc);
