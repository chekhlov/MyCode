#include "../stdafx.h"
#include "../include/HiScopePage.h"

HiscopePage::HiscopePage(std::shared_ptr<MainScreen> owner) : GamePage(owner)
{
};

void HiscopePage::Init()
{
	
	DeleteObjects();
	background = this->AddObject("resource\\Hiscopesbg.png", 1000);
	this->OnClick = IClickableObject::delegate::create<HiscopePage, &HiscopePage::OnClickClose>(this);
};

HiscopePage::~HiscopePage()
{
};

void HiscopePage::OnClickClose()
{
	auto mainScreen = (MainScreen *)owner.get();
	mainScreen->MainPage();
}


