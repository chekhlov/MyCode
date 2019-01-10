#include "../stdafx.h"
#include "../include/GameScreen.h"

GameScreen::GameScreen(std::shared_ptr<IGraphicEngine> owner) : GameObject()
{
	resource = owner;
};

GameScreen::~GameScreen()
{
	pages.clear();
};

std::shared_ptr<GamePage> GameScreen::AddPage()
{
	std::shared_ptr<GamePage>page(new GamePage(std::static_pointer_cast<GameScreen>(shared_from_this())));
	AddPage(page);
	return page;
}

void GameScreen::AddPage(std::shared_ptr<GamePage> page)
{
	AddObject(page);
	pages.push_back(page);

	if (ActivePage == nullptr) SetActivePage(page);
}

void GameScreen::SetActivePage(std::shared_ptr<GamePage> page)
{
//	AddObject(page);
//	pages.push_back(page);

//	ActivePage == nullptr) ActivePage = page;
}
//
//void GameScreen::Draw()
//{
//	// отрисовываем все дочерние объекты
//	resource->Clear();
//	for (auto object : objects)
//		object->Draw();
//
//	// отрисовываем себя на родителе
//	resource->Draw(Position->x, Position->y);
//}
