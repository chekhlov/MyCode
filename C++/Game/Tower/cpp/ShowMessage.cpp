#include "../stdafx.h"
#include "../helpers/ShowMessage.h"

int ShowMessage(std::wstring title, std::wstring text, UINT button)
{
	return MessageBoxW(NULL, text.c_str(), title.c_str(), button);
}

int ShowMessage(std::string title, std::string text, UINT button)
{
	return MessageBoxA(NULL, text.c_str(), title.c_str(), button);
}