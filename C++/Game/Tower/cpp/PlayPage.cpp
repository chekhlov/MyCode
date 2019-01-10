#include "../stdafx.h"
#include "../include/PlayPage.h"

PlayPage::PlayPage(std::shared_ptr<MainScreen> owner) : GamePage(owner)
{
};

void PlayPage::Init()
{
	DeleteObjects();

	background = this->AddObject("resource\\Playbg.png", 1000);
	sun = this->AddObject("resource\\Sun.png", 900);
	clouds1 = this->AddObject("resource\\clouds.png", 800);
	clouds2 = this->AddObject("resource\\clouds.png", 800);
//	bigBaloon = this->AddObject("resource\\BigBaloon.png", 100);
	smallBaloon = this->AddObject("resource\\SmallBaloon.png", 700);

	this->OnClick = IClickableObject::delegate::create<PlayPage, &PlayPage::OnClickToDrop>(this);
	clouds1->Position->x = Size->cx - clouds1->Size->cx;
	clouds2->Position->x = clouds1->Position->x - clouds2->Size->cx;

	sun->Position->x = 150;
	sun->Position->y = -90;
	smallBaloon->Position->x = 150;
	smallBaloon->Position->y = 20;
	droppedHouseBlock = nullptr;
	magnet = std::shared_ptr<Magnet>(new Magnet(std::static_pointer_cast<PlayPage>(shared_from_this())));
	magnet->Init();
	towers = std::shared_ptr<TowerBlock>(new TowerBlock(std::static_pointer_cast<PlayPage>(shared_from_this()), 100));
	towers->Init();
};

PlayPage::~PlayPage()
{
};

void PlayPage::OnClickToDrop()
{
	if (droppedHouseBlock != nullptr) return;

	droppedHouseBlock = magnet->DropHouse();
//	if (droppedHouseBlock == nullptr) return;

	// this->OnClick = IClickableObject::delegate::create<PlayPage, &PlayPage::OnClickClose>(this);
}

void PlayPage::OnClickClose()
{
	auto mainScreen = (MainScreen *)owner.get();
	mainScreen->MainPage();
}

void PlayPage::Tick(int tick)
{
	if (tick % 4 == 0)
	{
		clouds1->Position->x++;
		clouds2->Position->x++;
		if (clouds1->Position->x >= Size->cx)
			clouds1->Position->x = clouds2->Position->x - clouds1->Size->cx;

		if (clouds2->Position->x >= Size->cx)
			clouds2->Position->x = clouds1->Position->x - clouds2->Size->cx;
	}

	if (tick % 100 == 0)
	{
		sun->Position->y = -90 - (tick % 200) / 100;
	}

	magnet->Tick(tick);
	towers->Tick(tick);

	if (droppedHouseBlock != nullptr)
	{
		droppedHouseBlock->Tick(tick);
		DroppedBlockAnalyze();
	}
}

void PlayPage::DroppedBlockAnalyze()
{
	if (droppedHouseBlock->Position->y + droppedHouseBlock->Size->cy < towers->VisiblePosition->y + 30) return;

	towers->Add(droppedHouseBlock);
	droppedHouseBlock.reset();
	droppedHouseBlock = nullptr;
	magnet->Init();
}

