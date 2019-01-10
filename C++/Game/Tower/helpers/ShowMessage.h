#pragma once

#ifndef _SHOWMESSAGE_H_
#define _SHOWMESSAGE_H_

#include <windows.h>
#include <string>

int ShowMessage(std::wstring title, std::wstring text, UINT button = MB_OK | MB_ICONWARNING);
int ShowMessage(std::string title, std::string text, UINT button = MB_OK | MB_ICONWARNING);

#endif

