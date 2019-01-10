#include "../stdafx.h"
#include "../include/MainPage.h"
#include "../include/MainScreen.h"

MainPage::MainPage(std::shared_ptr<MainScreen> ownery) : GamePage(ownery)
{
};

void MainPage::Init()
{
	DeleteObjects();
	background = this->AddObject("resource\\mainbg.png", 1000);

	btPlay = this->AddButtonObject("resource\\btPlay.png");
	btPlay->AddFrame("resource\\btPlayFocus.png");
	btPlay->AddFrame("resource\\btPlayActive.png");
	btPlay->AddFrame("resource\\btPlayDown.png");
	SIZE size = Size;
	SIZE bt = btPlay->Size;
	int x = (Size->cx - btPlay->Size->cx) / 2;
	btPlay->Position->x =x;
	btPlay->Position->x = (Size->cx - btPlay->Size->cx) / 2;
	btPlay->Position->y = 170;

	btPlay->OnClick = IClickableObject::delegate::create<MainPage, &MainPage::OnClickPlay>(this);

	btHiscope = this->AddButtonObject("resource\\btHiscope.png");
	btHiscope->AddFrame("resource\\btHiscopeFocus.png");
	btHiscope->AddFrame("resource\\btHiscopeActive.png");
	btHiscope->AddFrame("resource\\btHiscopeDown.png");
	x = (Size->cx - btHiscope->Size->cx) / 2;
	btHiscope->Position->x = x;
	btHiscope->Position->x = (Size->cx - btHiscope->Size->cx) / 2;
	btHiscope->Position->y = 255;
	btHiscope->OnClick = IClickableObject::delegate::create<MainPage, &MainPage::OnClickHighscope>(this);

	btHowPlay = this->AddButtonObject("resource\\btHowPlay.png");
	btHowPlay->AddFrame("resource\\btHowPlayFocus.png");
	btHowPlay->AddFrame("resource\\btHowPlayActive.png");
	btHowPlay->AddFrame("resource\\btHowPlayDown.png");
	btHowPlay->Position->x = (Size->cx - btHiscope->Size->cx) / 2;
	btHowPlay->Position->y = 340;

	btHowPlay->OnClick = IClickableObject::delegate::create<MainPage, &MainPage::OnClickHowPlay>(this);
};

MainPage::~MainPage()
{
};


void MainPage::OnClickHowPlay()
{
	auto mainScreen = (MainScreen *) owner.get();
	mainScreen->HowPlayPage();
}

void MainPage::OnClickPlay()
{
	auto mainScreen = (MainScreen *)owner.get();
	mainScreen->PlayPage();
}

void MainPage::OnClickHighscope()
{
	auto mainScreen = (MainScreen *)owner.get();
	mainScreen->HiscopePage();
}

