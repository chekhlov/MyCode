#include "../stdafx.h"
#include "../include/MainScreen.h"
#include "../include/MainPage.h"
#include "../include/HowPlayPage.h"
#include "../include/HiScopePage.h"
#include "../include/PlayPage.h"

MainScreen::MainScreen(std::shared_ptr<IGraphicEngine> owner) : GameScreen(owner)
{
};

MainScreen::~MainScreen()
{
};

void MainScreen::Init()
{
	MainPage();
}

void MainScreen::MainPage()
{
	if (mainPage == nullptr)
	{
		mainPage = std::shared_ptr<::MainPage>(new ::MainPage(std::static_pointer_cast<MainScreen>(shared_from_this())));
		mainPage->Init();
	}
	SetPage(mainPage);
}

void MainScreen::HowPlayPage()
{
	if (howPlayPage == nullptr)
	{
		howPlayPage = std::shared_ptr<::HowPlayPage>(new ::HowPlayPage(std::static_pointer_cast<MainScreen>(shared_from_this())));
		howPlayPage->Init();
	}

	SetPage(howPlayPage);
}

void MainScreen::HiscopePage()
{
	if (hiscopePage == nullptr)
	{
		hiscopePage = std::shared_ptr<::HiscopePage>(new ::HiscopePage(std::static_pointer_cast<MainScreen>(shared_from_this())));
		hiscopePage->Init();
	}

	SetPage(hiscopePage);
}

void MainScreen::PlayPage()
{
	if (playPage == nullptr)
	{
		playPage = std::shared_ptr<::PlayPage>(new ::PlayPage(std::static_pointer_cast<MainScreen>(shared_from_this())));
		playPage->Init();
	}

	SetPage(playPage);
}

void MainScreen::ProcessEvent(int x, int y, MouseState state)
{
	if (mainScreenState == ssNormal)
		GameScreen::ProcessEvent(x, y, state);
}


void MainScreen::Tick(int tick)
{
	if (ActivePage != nullptr && mainScreenState == ssNormal)
		ActivePage->Tick(tick);

	if (mainScreenState == 	ssSwitchPage)
	{
		if (ActivePage->Position->x > 0)
		{
			int step = ActivePage->Size->cx / 10 + 1;
			ActivePage->Position->x -= step;
			prevPage->Position->x -= step;
		}

		if (ActivePage->Position->x < 0)
		{
			ActivePage->Position->x = 0;
			mainScreenState = ssNormal;
		}
	}
}

void MainScreen::SetPage(std::shared_ptr<GamePage> page)
{
	if (ActivePage == page) return;


	page->DeleteObjects();
	page->Init();

	if (!ExistsObject(page)) 
		AddObject(page, 0);

	prevPage = ActivePage;
	ActivePage = page;

	if (prevPage == nullptr)
	{ 
		mainScreenState = ssNormal;
		return;
	}

	AddObject(prevPage, 1);

	ActivePage->Position->x = prevPage->Size->cx;
	mainScreenState = ssSwitchPage;
}