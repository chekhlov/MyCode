#include "../stdafx.h"
#include "../include/TextObject.h"
#include "../include/GraphicEngine.h"
#include "../libs/sdl2/include/SDL_ttf.h"

TextObject::TextObject() : GameObject()
{
	FontSize(this);
	Color(this);
	FontName(this);
	Text(this);
	Allign(this);
}

TextObject::TextObject(std::shared_ptr<GameObject> owner, int fontSize, std::string fontName) : GameObject(owner)
{
	FontSize(this);
	Color(this);
	FontName(this);
	Text(this);
	Allign(this);

	this->text = text;
	this->fontName = fontName;
	this->fontSize = fontSize;

	auto object = owner->resource;
	if (!object)
		throw GameException("TextObject::TextObject Попытка создания непривязанного объекта");

	auto engine = object->GetEngine();
	if (!engine)
		throw GameException("TextObject::TextObject Неудалось получить ссылку на IGameEngine ");
}

TextObject::TextObject(std::shared_ptr<GameObject> owner, std::string text, int fontSize, std::string fontName) : TextObject(owner, fontSize, fontName)
{
	SetText(text);
}


TextObject::~TextObject()
{	
}

SDL_Color TextObject::GetColor()
{
	return this->fontColor;
}

SDL_Color TextObject::SetColor(SDL_Color const &color)
{
	this->fontColor = color;
	Update();
	return this->fontColor;
}

int TextObject::GetFontSize()
{
	return this->fontSize;
}

int TextObject::SetFontSize(int const &size)
{
	this->fontSize = size;
	Update();
	return this->fontSize;
}

std::string TextObject::GetFontName()
{
	return this->fontName;
}

std::string TextObject::SetFontName(std::string const &fontName)
{
	this->fontName = fontName;
	Update();
	return this->fontName;
}


std::string TextObject::GetText()
{
	return this->text;
} 

std::string TextObject::SetText(std::string const &text)
{
	this->text = text;
	Update();
	return this->text;
}

void TextObject::Update()
{
	resource->Free();
	
	if (text->empty()) return;


	TTF_Font* font = TTF_OpenFont(fontName->c_str(), fontSize);
	if (!font)
		throw GameException(fmt::format("TextObject::Update неудалось загрузить шрифт {0}\n{1}", (std::string) fontName, TTF_GetError()));

	SDL_Surface* surface = TTF_RenderUNICODE_Solid(font, (Uint16 *)widen(text).c_str(), fontColor); // as TTF_RenderText_Solid could only be used on SDL_Surface then you have to create the surface first
	if (!font)
		throw GameException(fmt::format("TextObject::Update неудалось создать поверхность\n{1}", TTF_GetError()));

	TTF_CloseFont(font);
	font = NULL;
	auto resource_ptr = std::static_pointer_cast<GraphicResource>(resource);
	resource_ptr->size = { surface->w, surface->h };

	auto render = std::static_pointer_cast<GraphicEngine>(resource_ptr->GetEngine())->GetRender();
	SDL_SetRenderDrawColor(render, 0x00, 0x00, 0x00, 0x00);
	SDL_RenderClear(render);
	resource_ptr->resource = SDL_CreateTextureFromSurface(render, surface); //now you can convert it into a texture
	SDL_FreeSurface(surface);

	if (textAlign) SetAlignment(textAlign);
}

TEXTOBJECT_ALIGNMENT TextObject::GetAlignment()
{
	return this->textAlign;
}
TEXTOBJECT_ALIGNMENT TextObject::SetAlignment(TEXTOBJECT_ALIGNMENT const &textAlign)
{
	this->textAlign = textAlign;
	POINT pos = { 0, 0 };

	int h = textAlign & 0x0F;
	int v = textAlign & 0xF0;

	auto objSize = GetSize();
	auto ownerSize = owner->GetSize();

	if (h == TEXTOBJECT_ALIGN_RIGHT)
		pos.x = ownerSize.cx - objSize.cx;

	if (h == TEXTOBJECT_ALIGN_CENTER)
		pos.x = (ownerSize.cx - objSize.cx) / 2;

	if (v == TEXTOBJECT_ALIGN_BOTTOM)
		pos.y = ownerSize.cy - objSize.cy;

	if (v == TEXTOBJECT_ALIGN_MIDDLE)
		pos.y = (ownerSize.cy - objSize.cy) / 2;

	Position = pos;

	return this->textAlign;
}
