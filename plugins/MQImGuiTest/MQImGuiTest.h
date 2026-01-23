#pragma once
#include <mq/Plugin.h>


//normal colors
ImVec4 black = MQColor(12, 12, 12).ToImColor();//
ImVec4 blue = MQColor(0, 0, 255).ToImColor();//
ImVec4 teal = MQColor(0, 255, 255).ToImColor();//
ImVec4 green = MQColor(0, 255, 0).ToImColor();//
ImVec4 magenta = MQColor(255, 0, 255).ToImColor();
ImVec4 orange = MQColor(255, 153, 0).ToImColor();//
ImVec4 purple = MQColor(128, 0, 128).ToImColor();
ImVec4 red = MQColor(255, 0, 0).ToImColor();//
ImVec4 white = MQColor(255, 255, 255).ToImColor();//
ImVec4 yellow = MQColor(255, 255, 0).ToImColor();//
//dark colors
ImVec4 black_dark = black;//
ImVec4 blue_dark = MQColor(0, 0, 153).ToImColor();//
ImVec4 teal_dark = MQColor(0, 153, 153).ToImColor();//
ImVec4 green_dark = MQColor(0, 153, 0).ToImColor();//
ImVec4 magenta_dark = MQColor(255, 0, 255).ToImColor();
ImVec4 orange_dark = MQColor(153, 102, 0).ToImColor();//
ImVec4 purple_dark = MQColor(153, 0, 153).ToImColor();//
ImVec4 red_dark = MQColor(153, 0, 0).ToImColor();
ImVec4 white_dark = MQColor(255, 255, 255).ToImColor();
ImVec4 yellow_dark = MQColor(153, 153, 0).ToImColor();//

struct CWTNLog {
	ImGuiTextBuffer Buffer;
	ImVector<int> LineOffsets;        // Index to lines offset
	bool ScrollToBottom;

	void Clear() {
		Buffer.clear();
	}

	void AddLog(char* fmt, ImVec4 color = ImVec4(1, 1, 1, 1), ...) {
		char buffer[MAX_STRING] = { 0 };
		sprintf_s(buffer, "%s%s", fmt, "\n");

		va_list args;
		va_start(args, buffer);
		Buffer.appendfv(buffer, args);
		va_end(args);
		ScrollToBottom = true;
	}

	void Draw() {
		if (ImGui::Button("Clear"))
			Clear();

		ImGui::Separator();
		ImGui::BeginChild("scrolling");
		ImGui::PushStyleVar(ImGuiStyleVar_ItemSpacing, ImVec2(0, 1));

		//put the text already!
		ImGui::TextUnformatted(Buffer.begin());

		if (ScrollToBottom) {
			ImGui::SetScrollHereY(ImGui::GetScrollMaxY());
			ScrollToBottom = false;
		}
		ImGui::PopStyleVar();
		ImGui::EndChild();
	}
};

void WriteImGuiChat(const char* in);