// MQImGuiTest.cpp : Defines the entry point for the DLL application.

//#include <mq/Plugin.h>//in the .h
#include <imgui.h>
#include <mq/imgui/ImGuiUtils.h>
#include <mq/imgui/Widgets.h>
#include <mq/imgui/ConsoleWidget.h>
#include "MQImGuiTest.h"

PreSetup("MQImGuiTest");
PLUGIN_VERSION(0.1);

/**
 * Avoid Globals if at all possible, since they persist throughout your program.
 * But if you must have them, here is the place to put them.
 */
// bool ShowMQImGuiTestWindow = true;
std::string pluginmsg = "\ar[\a-tMQImGuiTest\ar]\ao:: ";
std::string pluginname = "MQImGuiTest";
auto console = mq::imgui::ConsoleWidget::Create("##PluginConsole");
bool bDisplayGuiWindow = true;

void OutputCommand(PlayerClient* pChar, char* szLine) {
	if (gGameState != GAMESTATE_INGAME || !console)
		return;

	if (!strlen(szLine)) {
		console->AppendText("\arYou didn't provide any text to output");
		return;
	}

	if (console && bDisplayGuiWindow) {//Send to console if it's visible and created.
		console->AppendText(szLine, console->DEFAULT_COLOR, true);
	}
	else if (!bDisplayGuiWindow) {//Send to MQ Default window
		WriteChatf(szLine);
	}
}

void GuiTestCommand(PlayerClient* pChar, char* szLine) {
	if (gGameState != GAMESTATE_INGAME || !console)
		return;

	if (!strlen(szLine) && console) {
		char guioutput[MAX_STRING] = { 0 };
		sprintf_s(guioutput, "Window: %s", bDisplayGuiWindow ? "\arFalse" : "\agTrue");
		console->AppendText(guioutput);
		return;
	}

	if (!_stricmp(szLine, "show")) {
		bDisplayGuiWindow = !bDisplayGuiWindow;
		return;
	}
}

PLUGIN_API void InitializePlugin()
{
	//DebugSpewAlways("MQImGuiTest::Initializing version %f", MQ2Version);*/
	console->AppendText("\ayMQTestFanzyImGui\ar::\axInitializing version \ap0.1", MQColor(0,255,255).ToImColor(), true);

	AddCommand("/output", OutputCommand);
	AddCommand("/guitest", GuiTestCommand);
	// Examples:
	// AddXMLFile("MQUI_MyXMLFile.xml");
	// AddMQ2Data("mytlo", MyTLOData);
}

PLUGIN_API void ShutdownPlugin()
{
	RemoveCommand("/output");
	RemoveCommand("/guitest");
	// Examples:
	// RemoveXMLFile("MQUI_MyXMLFile.xml");
	// RemoveMQ2Data("mytlo");
}

PLUGIN_API bool OnIncomingChat(const char* Line, DWORD Color)
{
	MQColor logcolor = GetColorForChatColor(Color);
	char Output[MAX_STRING] = { 0 };
	switch (Color) {
		case 15://AA Experience cap?
		case 264:
		case 266://other hits you
		case 268:
		case 274://faction
		case 279:
		case 280:
		case 283://Your Spell Hits Other
		case 286:
		case 284:
		case 302://You heal pet/other?
		case 331:
		case 332:
		case 333://focus effects
		case 336:
		case 337://Our pet won't shut up
		case 328:
		case 343://Other dies (npc)
		case 358://You have slain
		case 363:
		case 364://others ds
		case 373://pet was healed
		case 380:
			break;
		default:
			sprintf_s(Output, "[%i] %s", Color, Line);
			console->AppendText(Output, logcolor, true);
			break;
	}
	//289 - You cannot see your target.
	return false;

}

/**
 * @fn OnUpdateImGui
 *
 * This is called each time that the ImGui Overlay is rendered. Use this to render
 * and update plugin specific widgets.
 *
 * Because this happens extremely frequently, it is recommended to move any actual
 * work to a separate call and use this only for updating the display.
 */

PLUGIN_API void OnUpdateImGui()
{
	if (!bDisplayGuiWindow)
		return;

	if (GetGameState() == GAMESTATE_INGAME) {
		ImGuiTextFilter Filter;
		ImGui::SetNextWindowSize(ImVec2(400, 440), ImGuiCond_FirstUseEver);
		ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 10);
		ImGui::PushStyleVar(ImGuiStyleVar_ScrollbarRounding, 50);
		ImGui::PushStyleVar(ImGuiStyleVar_FrameRounding, 5);
		int stylevars = 3;

		ImGuiWindowFlags flags = ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoDocking;
		ImGuiTabBarFlags tabflags = ImGuiTabBarFlags_Reorderable | ImGuiTabBarFlags_NoCloseWithMiddleMouseButton | ImGuiTabBarFlags_FittingPolicyScroll;

		if (ImGui::Begin("ClassPlugin", &bDisplayGuiWindow, flags)) {
			if (ImGui::BeginMenuBar()) {
				WriteImGuiChat( "\arMQImGuiTest Initialized!");
				ImGui::EndMenuBar();
			}

			if (ImGui::BeginTabBar("Plugin Output", tabflags)) {
				if (ImGui::BeginTabItem("Plugin Output")) {
					console->Render();//Render the Plugin Output. Should be all we need in here.

					ImGui::EndTabItem();
				}

				if (ImGui::BeginTabItem("Test output")) {
					if (!ImGui::BeginChild(123123, ImGui::GetContentRegionAvail(), true, flags)) {
						ImGui::EndChild();
					}
					else {
						WriteImGuiChat("\arHiya, what's the word man");

						ImGui::EndChild();
					}




					ImGui::EndTabItem();
				}

				ImGui::EndTabBar();
			}
		}
		ImGui::PopStyleVar(stylevars);
		ImGui::End();

	}
}

/*
* Safe to ignore stuff under this line for now.
* Don't want to re-invent the wheel, but also not ready to dispose of this yet.
* It could have uses for labels and titles etc that don't require formatted strings.
*/

//Does not format text that has %s, %f etc in it.
void WriteImGuiChat(const char* in) {
	size_t i = 0;
	size_t len = strlen(in);
	std::vector<ImVec4> color;
	color.push_back(white);
	ImVec4 prevcolor = color.back();
	std::string buffer;
	while (i < len && in[i]) {
		if (in[i] == '\a') {
			//This is the start of a color, but which one?
			// - /ay (yellow) /ao (orange) etc
			i++;
			bool dark = false;
			if (in[i] == '-') {//if it's a dash, it's dark colors.
				dark = true;
				i++;
			}

			switch (in[i]) {
				case 'b'://ab - Black
					color.push_back(black);
					break;
				case 'u'://au - Blue
					dark ? color.push_back(blue_dark) : color.push_back(blue);
					break;
				case 't'://at - Teal
					dark ? color.push_back(teal_dark) : color.push_back(teal);
					break;
				case 'g'://ag - Green
					dark ? color.push_back(green_dark) : color.push_back(green);
					break;
				case 'm'://am - Magenta
					dark ? color.push_back(magenta_dark) : color.push_back(magenta);
					break;
				case 'o'://ao - Orange
					dark ? color.push_back(orange_dark) : color.push_back(orange);
					break;
				case 'p'://ap - Purple
					dark ? color.push_back(purple_dark) : color.push_back(purple);
					break;
				case 'r'://ar - Red
					dark ? color.push_back(red_dark) : color.push_back(red);
					break;
				case 'w'://aw - White
					dark ? color.push_back(white_dark) : color.push_back(white);
					break;
				case 'y'://ay - Yellow
					dark ? color.push_back(yellow_dark) : color.push_back(yellow);
					break;
				case 'x'://ax previous color
					if (!color.empty()) {
						color.pop_back();
					}
					break;
				default://what you doing here willis? probably a typo? Should I let us know?
					break;
			}

		}
		else if (in[i] != '\n') {
			if (color.back().x != prevcolor.x || color.back().y != prevcolor.y || color.back().z != prevcolor.z || color.back().w != prevcolor.w) {
				if (!buffer.empty()) {
					ImGui::PushStyleColor(ImGuiCol_Text, prevcolor);
					ImGui::TextWrapped(buffer.c_str());
					ImGui::PopStyleColor();
					buffer.clear();
				}
				prevcolor = color.back();
			}

			buffer += in[i];
		}

		if (i >= len) {
			if (!buffer.empty()) {
				ImGui::TextColored(color.back(), buffer.c_str());
				buffer.clear();
				if (i < len) {//TextColored adds a space, so if our next character is a space, skip it.
					if (in[i + 1] == ' ')
						i++;
				}
			}

			break;
		}
		i++;
	}
}


PLUGIN_API void OnBeginZone() {
	// DebugSpewAlways("MQImGuiTest::OnBeginZone()");
}

/**
 * @fn OnZoned
 *
 * This is called after entering a new zone and the zone is considered "loaded."
 *
 * It occurs after @ref OnEndZone @ref OnAddSpawn and @ref OnAddGroundItem have
 * been called.
 */
PLUGIN_API void OnZoned() {
	// DebugSpewAlways("MQImGuiTest::OnZoned()");
}

PLUGIN_API void OnCleanUI() {
	// DebugSpewAlways("MQImGuiTest::OnCleanUI()");
}

PLUGIN_API void OnReloadUI() {
	// DebugSpewAlways("MQImGuiTest::OnReloadUI()");
}