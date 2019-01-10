#include "../stdafx.h"
#include "../include/HowPlayPage.h"

HowPlayPage::HowPlayPage(std::shared_ptr<MainScreen> owner) : GamePage(owner)
{
};

void HowPlayPage::Init()
{
	DeleteObjects();
	background = this->AddObject("resource\\howtoplaybg.png", 1000);
	this->OnClick = IClickableObject::delegate::create<HowPlayPage, &HowPlayPage::OnClickClose>(this);

};

HowPlayPage::~HowPlayPage()
{
};

void HowPlayPage::OnClickClose()
{
	auto mainScreen = (MainScreen *)owner.get();
	mainScreen->MainPage();
}


