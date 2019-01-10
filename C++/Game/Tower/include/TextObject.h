#pragma once
#ifndef _TEXTOBJECT_H_
#define _TEXTOBJECT_H_

#include "windows.h"
#include "interface.h"
#include "GameObject.h"
#include "../helpers/property.h"

typedef enum TEXTOBJECT_ALIGNMENT {
	TEXTOBJECT_ALIGN_LEFT = 0x00,
	TEXTOBJECT_ALIGN_CENTER = 0x01,
	TEXTOBJECT_ALIGN_RIGHT = 0x02,
	TEXTOBJECT_ALIGN_TOP = 0x00,
	TEXTOBJECT_ALIGN_MIDDLE = 0x10,
	TEXTOBJECT_ALIGN_BOTTOM = 0x20
} TEXTOBJECT_ALIGNMENT;

DEFINE_ENUM_FLAG_OPERATORS(TEXTOBJECT_ALIGNMENT);


class TextObject : public GameObject
{
private:
protected:
	Property <std::string> text;
	Property <std::string> fontName;
	Property<int> fontSize;
	Property <SDL_Color> fontColor;
	Property <TEXTOBJECT_ALIGNMENT> textAlign;
	virtual int GetFontSize();
	virtual int SetFontSize(int const &size);
	virtual SDL_Color GetColor();
	virtual SDL_Color SetColor(SDL_Color const &color);
	virtual std::string GetText();
	virtual std::string SetText(std::string const &text);
	virtual std::string GetFontName();
	virtual std::string SetFontName(std::string const &text);
	virtual TEXTOBJECT_ALIGNMENT GetAlignment();
	virtual TEXTOBJECT_ALIGNMENT SetAlignment(TEXTOBJECT_ALIGNMENT const &align);
	virtual void Update();
	TextObject();
public:
	RWProperty<int, TextObject, &TextObject::GetFontSize, &TextObject::SetFontSize> FontSize;
	RWProperty<SDL_Color, TextObject, &TextObject::GetColor, &TextObject::SetColor> Color;
	RWProperty<std::string, TextObject, &TextObject::GetFontName, &TextObject::SetFontName> FontName;
	RWProperty<std::string, TextObject, &TextObject::GetText, &TextObject::SetText> Text;
	RWProperty<TEXTOBJECT_ALIGNMENT, TextObject, &TextObject::GetAlignment, &TextObject::SetAlignment> Allign;

	TextObject(std::shared_ptr<GameObject> owner, int fontSize = 12, std::string fontName = "FreeSans.ttf");
	TextObject(std::shared_ptr<GameObject> owner, std::string text, int fontSize = 12, std::string fontName = "FreeSans.ttf");
	virtual ~TextObject();
	inline virtual std::string operator = (std::string text) { SetText(text); return this->text; };
	inline virtual operator std::string () { return GetText(); };
};

#endif
