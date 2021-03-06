// Tower.cpp: Определяет точку входа для приложения.
//

#include "stdafx.h"
#include "Tower.h"
#include "helpers/ShowMessage.h"
#include "include/GraphicEngine.h"
#include "include/Game.h"


int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR    lpCmdLine, _In_ int       nCmdShow)
{
		try
		{
			std::shared_ptr<Game> game(new Game(320, 480));
			game->Execute();
		}
		catch(GameException &ex)
		{
			ShowMessage("Ошибка!", fmt::format("В программе произошла ошибка.\nДетали: {0}", ex.what()));
		}
		catch(std::exception &ex)
		{
			ShowMessage("Неизвестная ошибка!", ex.what());
		}

	return 0;
}
