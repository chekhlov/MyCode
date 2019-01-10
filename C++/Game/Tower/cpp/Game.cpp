#include "../stdafx.h"
#include "../include/Game.h"
#include <mmsystem.h>
#include "../include/MainScreen.h"
#pragma comment(lib, "winmm.lib")

Game::Game(int cx, int cy)
{
	windowSize.cx = cx;
	windowSize.cy = cy;
	InitEngine(cx, cy);
}

Game::~Game()
{
	if (engine != nullptr) engine->Free();
}

void Game::InitEngine(int cx, int cy)
{
	if (cx <= 0) cx = windowSize.cx;
	if (cy <= 0) cy = windowSize.cy;
	if (engine != nullptr) engine->Free();
	engine = std::shared_ptr<IGraphicEngine>(new GraphicEngine(cx, cy));
	engine->Init(cx, cy);
	mainScreen = PrepareMainScreen(engine);
}

std::shared_ptr<IGraphicEngine> Game::GetEngine()
{
	if (engine == nullptr) InitEngine();
	return engine;
}

int Game::Execute()
{

	quit = false;
	while (!quit) 
	{
		SDL_Event event;


		while (SDL_PollEvent(&event) && !quit)
		{
			SDL_PumpEvents(); // обработчик событий.
 			ProcessEvent(event);
		};
		Sync();
	}

	return 0;
}

void Game::ProcessEvent(SDL_Event &event)
{

	switch (event.type)
	{
		case SDL_QUIT: quit = true; break;
		case SDL_MOUSEBUTTONDOWN:
			mainScreen->ProcessEvent(event.button.x, event.button.y, event.button.button == SDL_BUTTON_LEFT
				? IClickableObject::mbtLeftDown 
				: IClickableObject::mbtRightDown);
			break;
		case SDL_MOUSEBUTTONUP:
			mainScreen->ProcessEvent(event.button.x, event.button.y, event.button.button == SDL_BUTTON_LEFT
				? IClickableObject::mbtLeftUp
				: IClickableObject::mbtRightUp);
			break;
		case SDL_MOUSEMOTION:
			mainScreen->ProcessEvent(event.motion.x, event.motion.y, IClickableObject::mbtMove);
			break;
	}

}

void Game::Exit()
{
	quit = true;
}

float Game::CalcFps()
{
	static int framesPerSecond = 0;
	static float lastTime = 0.0f;  
	//получаем текущий tick count и умножаем его на 0.001 для конвертации из миллисекунд в секунды.
	float currentTime = timeGetTime() * 0.001f;

	framesPerSecond++;

	if (currentTime - lastTime > 1.0f)
	{
		lastTime = currentTime;
		fps = framesPerSecond;
		framesPerSecond = 0;
	}
	return fps;
}

void Game::Sync()
{
	CalcFps();
	*frameText = fmt::format("fps: {0}, frame: {1}", fps, tick);
	tick++;
	mainScreen->Tick(tick);
	mainScreen->Draw();
	engine->SwitchRender();
	Sleep(20);
}

std::shared_ptr<GameScreen> Game::PrepareMainScreen(std::shared_ptr<IGraphicEngine> engine)
{
	mainScreen = std::shared_ptr<GameScreen>(new MainScreen(engine));
	mainScreen->Init();
	frameText = mainScreen->AddTextObject(-100);
	frameText->Color = {255,255,255, 0};
	frameText->Allign = TEXTOBJECT_ALIGN_CENTER;


	// Тупой способ сделать толстый шрифт - времени не хватает
	auto aboutText = mainScreen->AddTextObject(-100);
	aboutText->Text = "2018 Тестовое задание - Чехлов Александр";
	aboutText->FontSize = 14;
	aboutText->Color = { 0, 0, 0, 0 };
	aboutText->Allign = TEXTOBJECT_ALIGN_CENTER | TEXTOBJECT_ALIGN_BOTTOM;
	aboutText->Position->x++;

	aboutText = mainScreen->AddTextObject(-100);
	aboutText->Text = "2018 Тестовое задание - Чехлов Александр";
	aboutText->FontSize = 14;
	aboutText->Color = { 0, 0, 0, 0 };
	aboutText->Allign = TEXTOBJECT_ALIGN_CENTER | TEXTOBJECT_ALIGN_BOTTOM;
	aboutText->Position->x--;

	aboutText = mainScreen->AddTextObject(-100);
	aboutText->Text = "2018 Тестовое задание - Чехлов Александр";
	aboutText->FontSize = 14;
	aboutText->Color = { 0, 0, 0, 0 };
	aboutText->Allign = TEXTOBJECT_ALIGN_CENTER | TEXTOBJECT_ALIGN_BOTTOM;
	aboutText->Position->y--;

	aboutText = mainScreen->AddTextObject(-100);
	aboutText->Text = "2018 Тестовое задание - Чехлов Александр";
	aboutText->FontSize = 14;
	aboutText->Color = { 0, 0, 0, 0 };
	aboutText->Allign = TEXTOBJECT_ALIGN_CENTER | TEXTOBJECT_ALIGN_BOTTOM;
	aboutText->Position->y++;

	aboutText = mainScreen->AddTextObject(-100);
	aboutText->Text = "2018 Тестовое задание - Чехлов Александр";
	aboutText->FontSize = 14;
	aboutText->Color = { 255, 255, 255, 0 };
	aboutText->Allign = TEXTOBJECT_ALIGN_CENTER | TEXTOBJECT_ALIGN_BOTTOM;

//	textObject->SetResource(shared_ptr<TextObject>(new TextObject(page, "Проверка вывода текста", 15)));
//	textObject->SetPosition(GAMEOBJECT_ALLIGN_CENTER);
	return mainScreen;
}