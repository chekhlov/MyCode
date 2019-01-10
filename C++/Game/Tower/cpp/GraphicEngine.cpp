#include "../stdafx.h"
#include "../include/GraphicEngine.h"
#include "../include/GraphicResource.h"
#include "../libs/sdl2/include/SDL_ttf.h"

SIZE GraphicEngine::GetMaximumScreenSize()
{
	SIZE size;
	size.cx = maxScreenSizeX;
	size.cy = maxScreenSizeY;
	return size;
}

SIZE GraphicEngine::GetMinimumScreenSize()
{
	SIZE size;
	size.cx = minScreenSizeX;
	size.cy = minScreenSizeY;
	return size;
}

GraphicEngine::GraphicEngine(int sizeX, int sizeY)
{
	Size(this);

	size.cx = sizeX;
	size.cy = sizeY;
}

GraphicEngine::~GraphicEngine()
{
	Free();
}

void GraphicEngine::Free()
{
	isInit = true;
	if (render) SDL_DestroyRenderer(render);
	render = nullptr;
	if (window) SDL_DestroyWindow(window);
	window = nullptr;
	TTF_Quit();
	SDL_Quit();
}

void GraphicEngine::Init(int sizeX, int sizeY)
{
	auto maxSize = GetMaximumScreenSize();
	if (sizeX == -1) sizeX = maxSize.cx;
	if (sizeY == -1) sizeY = maxSize.cy;

	if (sizeX > maxSize.cx)
		throw GameException(fmt::format("GraphicEngine::Init Недопустимый параметр sizeX.\nПревышен размер экрана - current {0}, max {1}", sizeX, maxSize.cx));

	if (sizeX > maxSize.cx)
		throw GameException(fmt::format("GraphicEngine::Init Недопустимый параметр sizeY.\nПревышен размер экрана - current {0}, max {1}", sizeY, maxSize.cy));

	auto minSize = GetMinimumScreenSize();

	if (sizeY < minSize.cx)
		throw GameException(fmt::format("GraphicEngine::Init Недопустимый параметр sizeX.\nУказан размер экрана - current {0}, min {1}", sizeX, minSize.cx));

	if (sizeY < minSize.cx)
		throw GameException(fmt::format("GraphicEngine::Init Недопустимый параметр sizeY.\nУказан размер экрана - current {0}, min {1}", sizeY, minSize.cy));

	// Инициализация библиотеки SDL
	if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER) < 0)
		throw GameException(fmt::format("GraphicEngine::GetRender Ошибка инициализации SDL: {0}", SDL_GetError()));

	window = SDL_CreateWindow("The game", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, sizeX, sizeY, SDL_WINDOW_SHOWN);
	if (!window)
		throw GameException(fmt::format("GraphicEngine::GetRender Ошибка создания окна: {0}", SDL_GetError()));

	render = SDL_CreateRenderer(window, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
	if (render == nullptr)
		throw GameException(fmt::format("GraphicEngine::GetRender Ошибка создания рендера: {0}", SDL_GetError()));

	TTF_Init();

	isInit = true;
}

SDL_Renderer* GraphicEngine::GetRender()
{
	if (!isInit) Init(size.cx, size.cy);
	if (render == nullptr)
		throw GameException("GraphicEngine::GetRender Ошибка вызова.\nРендер не был проинициализирован.");
	return render;
}

std::shared_ptr<IGraphicResource> GraphicEngine::LoadResource(std::shared_ptr<IGraphicResource> owner, std::string fileName)
{
	std::shared_ptr<IGraphicResource>res(new GraphicResource(owner));
	res->LoadResource(fileName);
	return res;
}

SIZE GraphicEngine::GetSize()
{
	return size;
}

std::shared_ptr<IGraphicEngine> GraphicEngine::GetEngine()
{
	return std::static_pointer_cast<IGraphicEngine>(shared_from_this());
}

void GraphicEngine::Clear()
{
	if (!isInit)
		throw GameException("GraphicEngine::PrepairRender GraphicEngine не был проинициализирован.");

	SDL_RenderClear(render);
}

void GraphicEngine::Draw(SDL_Rect *dstRect)
{
	// заглушки - родителя нет :)
}

void GraphicEngine::Draw(int x, int y, int cx, int cy)
{
	// заглушки - родителя нет :)
}


void GraphicEngine::Draw(IGraphicResource *object, SDL_Rect *dstRect, SDL_Rect *srcRect)
{
	if (!isInit)
		throw GameException("GraphicEngine::Draw GraphicEngine не был проинициализирован.");

	auto resource = (GraphicResource *)object;
	if (!resource) return;

	SIZE size = object->GetSize();
	if (dstRect->w <= 0) dstRect->w = size.cx;
	if (dstRect->h <= 0) dstRect->h = size.cy;

//	dstRect = SDL_RectEmpty(dstRect) ? NULL : dstRect;

	SDL_RenderCopy(render, resource->resource, srcRect, dstRect);
}

void GraphicEngine::Draw(std::shared_ptr<IGraphicResource> object, SDL_Rect *dstRect, SDL_Rect *srcRect)
{
	Draw(object.get(), dstRect, srcRect);
}

void GraphicEngine::Draw(std::shared_ptr<IGraphicResource> object, int x, int y, int cx, int cy)
{
	SIZE size = object->GetSize();
	if (cx <= 0) cx = size.cx;
	if (cy <= 0) cy = size.cy;

	SDL_Rect dRect = { x, y, cx, cy };
	SDL_Rect *dstRect = SDL_RectEmpty(&dRect) ? NULL : &dRect;

	Draw(object.get(), dstRect, NULL);
}

void GraphicEngine::FreeResource(std::shared_ptr<IGraphicResource> object)
{
	object.reset();
}

void GraphicEngine::FreeResource(IGraphicResource *object)
{
	auto resource = (GraphicResource *) object;
	if (!resource) return;

	SDL_DestroyTexture(resource->resource);
	resource->resource = nullptr;
}

void GraphicEngine::PrepairRender()
{
	Clear();
}

void GraphicEngine::SwitchRender()
{
	if (!isInit)
		throw GameException("GraphicEngine::SwitchRender GraphicEngine не был проинициализирован.");

	SDL_RenderPresent(render);
	PrepairRender();
}


