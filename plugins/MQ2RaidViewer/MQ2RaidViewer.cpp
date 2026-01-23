// MQ2RaidViewer.cpp : Defines the entry point for the DLL application.
//

// PLUGIN_API is only to be used for callbacks.  All existing callbacks at this time
// are shown below. Remove the ones your plugin does not use.  Always use Initialize
// and Shutdown for setup and cleanup.

#include <mq/Plugin.h>

PreSetup("MQ2RaidViewer");
PLUGIN_VERSION(0.1);

/**
 * Avoid Globals if at all possible, since they persist throughout your program.
 * But if you must have them, here is the place to put them.
 */
// bool ShowMQ2RaidViewerWindow = true;

/**
 * @fn InitializePlugin
 *
 * This is called once on plugin initialization and can be considered the startup
 * routine for the plugin.
 */
PLUGIN_API void InitializePlugin()
{
	DebugSpewAlways("MQ2RaidViewer::Initializing version %f", MQ2Version);

	// Examples:
	// AddCommand("/mycommand", MyCommand);
	// AddXMLFile("MQUI_MyXMLFile.xml");
	// AddMQ2Data("mytlo", MyTLOData);
}

/**
 * @fn ShutdownPlugin
 *
 * This is called once when the plugin has been asked to shutdown.  The plugin has
 * not actually shut down until this completes.
 */
PLUGIN_API void ShutdownPlugin()
{
	DebugSpewAlways("MQ2RaidViewer::Shutting down");

	// Examples:
	// RemoveCommand("/mycommand");
	// RemoveXMLFile("MQUI_MyXMLFile.xml");
	// RemoveMQ2Data("mytlo");
}

/**
 * @fn OnCleanUI
 *
 * This is called once just before the shutdown of the UI system and each time the
 * game requests that the UI be cleaned.  Most commonly this happens when a
 * /loadskin command is issued, but it also occurs when reaching the character
 * select screen and when first entering the game.
 *
 * One purpose of this function is to allow you to destroy any custom windows that
 * you have created and cleanup any UI items that need to be removed.
 */
PLUGIN_API void OnCleanUI()
{
	// DebugSpewAlways("MQ2RaidViewer::OnCleanUI()");
}

/**
 * @fn OnReloadUI
 *
 * This is called once just after the UI system is loaded. Most commonly this
 * happens when a /loadskin command is issued, but it also occurs when first
 * entering the game.
 *
 * One purpose of this function is to allow you to recreate any custom windows
 * that you have setup.
 */
PLUGIN_API void OnReloadUI()
{
	// DebugSpewAlways("MQ2RaidViewer::OnReloadUI()");
}

/**
 * @fn OnDrawHUD
 *
 * This is called each time the Heads Up Display (HUD) is drawn.  The HUD is
 * responsible for the net status and packet loss bar.
 *
 * Note that this is not called at all if the HUD is not shown (default F11 to
 * toggle).
 *
 * Because the net status is updated frequently, it is recommended to have a
 * timer or counter at the start of this call to limit the amount of times the
 * code in this section is executed.
 */
PLUGIN_API void OnDrawHUD()
{
/*
	static std::chrono::steady_clock::time_point DrawHUDTimer = std::chrono::steady_clock::now();
	// Run only after timer is up
	if (std::chrono::steady_clock::now() > DrawHUDTimer)
	{
		// Wait half a second before running again
		DrawHUDTimer = std::chrono::steady_clock::now() + std::chrono::milliseconds(500);
		DebugSpewAlways("MQ2RaidViewer::OnDrawHUD()");
	}
*/
}

/**
 * @fn SetGameState
 *
 * This is called when the GameState changes.  It is also called once after the
 * plugin is initialized.
 *
 * For a list of known GameState values, see the constants that begin with
 * GAMESTATE_.  The most commonly used of these is GAMESTATE_INGAME.
 *
 * When zoning, this is called once after @ref OnBeginZone @ref OnRemoveSpawn
 * and @ref OnRemoveGroundItem are all done and then called once again after
 * @ref OnEndZone and @ref OnAddSpawn are done but prior to @ref OnAddGroundItem
 * and @ref OnZoned
 *
 * @param GameState int - The value of GameState at the time of the call
 */
PLUGIN_API void SetGameState(int GameState)
{
	// DebugSpewAlways("MQ2RaidViewer::SetGameState(%d)", GameState);
}


/**
 * @fn OnPulse
 *
 * This is called each time MQ2 goes through its heartbeat (pulse) function.
 *
 * Because this happens very frequently, it is recommended to have a timer or
 * counter at the start of this call to limit the amount of times the code in
 * this section is executed.
 */
PLUGIN_API void OnPulse()
{
/*
	static std::chrono::steady_clock::time_point PulseTimer = std::chrono::steady_clock::now();
	// Run only after timer is up
	if (std::chrono::steady_clock::now() > PulseTimer)
	{
		// Wait 5 seconds before running again
		PulseTimer = std::chrono::steady_clock::now() + std::chrono::seconds(5);
		DebugSpewAlways("MQ2RaidViewer::OnPulse()");
	}
*/
}

/**
 * @fn OnWriteChatColor
 *
 * This is called each time WriteChatColor is called (whether by MQ2Main or by any
 * plugin).  This can be considered the "when outputting text from MQ" callback.
 *
 * This ignores filters on display, so if they are needed either implement them in
 * this section or see @ref OnIncomingChat where filters are already handled.
 *
 * If CEverQuest::dsp_chat is not called, and events are required, they'll need to
 * be implemented here as well.  Otherwise, see @ref OnIncomingChat where that is
 * already handled.
 *
 * For a list of Color values, see the constants for USERCOLOR_.  The default is
 * USERCOLOR_DEFAULT.
 *
 * @param Line const char* - The line that was passed to WriteChatColor
 * @param Color int - The type of chat text this is to be sent as
 * @param Filter int - (default 0)
 */
PLUGIN_API void OnWriteChatColor(const char* Line, int Color, int Filter)
{
	// DebugSpewAlways("MQ2RaidViewer::OnWriteChatColor(%s, %d, %d)", Line, Color, Filter);
}

/**
 * @fn OnIncomingChat
 *
 * This is called each time a line of chat is shown.  It occurs after MQ filters
 * and chat events have been handled.  If you need to know when MQ2 has sent chat,
 * consider using @ref OnWriteChatColor instead.
 *
 * For a list of Color values, see the constants for USERCOLOR_. The default is
 * USERCOLOR_DEFAULT.
 *
 * @param Line const char* - The line of text that was shown
 * @param Color int - The type of chat text this was sent as
 *
 * @return bool - Whether to filter this chat from display
 */
PLUGIN_API bool OnIncomingChat(const char* Line, DWORD Color)
{
	// DebugSpewAlways("MQ2RaidViewer::OnIncomingChat(%s, %d)", Line, Color);
	return false;
}

/**
 * @fn OnAddSpawn
 *
 * This is called each time a spawn is added to a zone (ie, something spawns). It is
 * also called for each existing spawn when a plugin first initializes.
 *
 * When zoning, this is called for all spawns in the zone after @ref OnEndZone is
 * called and before @ref OnZoned is called.
 *
 * @param pNewSpawn PSPAWNINFO - The spawn that was added
 */
PLUGIN_API void OnAddSpawn(PSPAWNINFO pNewSpawn)
{
	// DebugSpewAlways("MQ2RaidViewer::OnAddSpawn(%s)", pNewSpawn->Name);
}

/**
 * @fn OnRemoveSpawn
 *
 * This is called each time a spawn is removed from a zone (ie, something despawns
 * or is killed).  It is NOT called when a plugin shuts down.
 *
 * When zoning, this is called for all spawns in the zone after @ref OnBeginZone is
 * called.
 *
 * @param pSpawn PSPAWNINFO - The spawn that was removed
 */
PLUGIN_API void OnRemoveSpawn(PSPAWNINFO pSpawn)
{
	// DebugSpewAlways("MQ2RaidViewer::OnRemoveSpawn(%s)", pSpawn->Name);
}

/**
 * @fn OnAddGroundItem
 *
 * This is called each time a ground item is added to a zone (ie, something spawns).
 * It is also called for each existing ground item when a plugin first initializes.
 *
 * When zoning, this is called for all ground items in the zone after @ref OnEndZone
 * is called and before @ref OnZoned is called.
 *
 * @param pNewGroundItem PGROUNDITEM - The ground item that was added
 */
PLUGIN_API void OnAddGroundItem(PGROUNDITEM pNewGroundItem)
{
	// DebugSpewAlways("MQ2RaidViewer::OnAddGroundItem(%d)", pNewGroundItem->DropID);
}

/**
 * @fn OnRemoveGroundItem
 *
 * This is called each time a ground item is removed from a zone (ie, something
 * despawns or is picked up).  It is NOT called when a plugin shuts down.
 *
 * When zoning, this is called for all ground items in the zone after
 * @ref OnBeginZone is called.
 *
 * @param pGroundItem PGROUNDITEM - The ground item that was removed
 */
PLUGIN_API void OnRemoveGroundItem(PGROUNDITEM pGroundItem)
{
	// DebugSpewAlways("MQ2RaidViewer::OnRemoveGroundItem(%d)", pGroundItem->DropID);
}

/**
 * @fn OnBeginZone
 *
 * This is called just after entering a zone line and as the loading screen appears.
 */
PLUGIN_API void OnBeginZone()
{
	// DebugSpewAlways("MQ2RaidViewer::OnBeginZone()");
}

/**
 * @fn OnEndZone
 *
 * This is called just after the loading screen, but prior to the zone being fully
 * loaded.
 *
 * This should occur before @ref OnAddSpawn and @ref OnAddGroundItem are called. It
 * always occurs before @ref OnZoned is called.
 */
PLUGIN_API void OnEndZone()
{
	// DebugSpewAlways("MQ2RaidViewer::OnEndZone()");
}

/**
 * @fn OnZoned
 *
 * This is called after entering a new zone and the zone is considered "loaded."
 *
 * It occurs after @ref OnEndZone @ref OnAddSpawn and @ref OnAddGroundItem have
 * been called.
 */
PLUGIN_API void OnZoned()
{
	// DebugSpewAlways("MQ2RaidViewer::OnZoned()");
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
bool ShowMQ2RaidViewerWindow = true;
ImVec4 grey = MQColor(37, 37, 37).ToImColor();//
ImVec4 black = MQColor(12, 12, 12).ToImColor();//
ImVec4 grey_dark = MQColor(28, 28, 28).ToImColor();//
ImVec4 blue_dark = MQColor(0, 0, 153).ToImColor();//
ImVec4 yellow_dark = MQColor(153, 153, 0).ToImColor();//
ImVec4 red_dark = MQColor(230, 0, 0).ToImColor();//
PLUGIN_API void OnUpdateImGui()
{

	if (GetGameState() == GAMESTATE_INGAME)
	{
		if (!pLocalPlayer)
			return;


		if (pRaid && pRaid->RaidMemberCount)
		{
			ImGui::PushStyleVar(ImGuiStyleVar_WindowRounding, 10.0f);
			ImGui::PushStyleVar(ImGuiStyleVar_ScrollbarRounding, 50.0f);
			ImGui::PushStyleVar(ImGuiStyleVar_FrameRounding, 5.0f);
			ImGui::PushStyleVar(ImGuiStyleVar_WindowMinSize, ImVec2(200.0f, 150.0f));
			ImGui::PushStyleVar(ImGuiStyleVar_TabRounding, 9);
			// 5 push style Var; if we add more, make sure we pop
			const int iPushPopVar = 5;
			ImGuiWindowFlags windowFlags = ImGuiWindowFlags_NoNav | ImGuiWindowFlags_NoFocusOnAppearing;

			ImGui::PushStyleColor(ImGuiCol_FrameBg, grey);
			ImGui::PushStyleColor(ImGuiCol_FrameBgHovered, black);
			ImGui::PushStyleColor(ImGuiCol_Tab, black);
			ImGui::PushStyleColor(ImGuiCol_TabActive, grey);
			ImGui::PushStyleColor(ImGuiCol_TabHovered, grey);
			ImGui::PushStyleColor(ImGuiCol_TitleBgActive, grey);
			ImGui::PushStyleColor(ImGuiCol_Button, grey);
			ImGui::PushStyleColor(ImGuiCol_ButtonActive, grey);
			ImGui::PushStyleColor(ImGuiCol_ButtonHovered, grey_dark);
			ImGui::PushStyleColor(ImGuiCol_ResizeGrip, grey);
			//Header is for Listbox (Ignore/Immune lists)
			ImGui::PushStyleColor(ImGuiCol_Header, black);
			ImGui::PushStyleColor(ImGuiCol_HeaderHovered, black);
			ImGui::PushStyleColor(ImGuiCol_HeaderActive, black);
			// 13 push style Color; if we add more, make sure we pop
			const int iPushPopColor = 13;

			if (!ImGui::Begin("MQ2RaidViewer", &ShowMQ2RaidViewerWindow, windowFlags)) {
				ImGui::PopStyleVar(iPushPopVar);
				ImGui::PopStyleColor(iPushPopColor);
				ImGui::End();

				return;
			}


			ImGuiTableFlags tableFlags = ImGuiTableFlags_Borders;
			ImGui::BeginTable("##RaidTable", 1, tableFlags);
			//cycle raid members here in loop.
			for (int i = 0; i < MAX_RAID_SIZE; i++) {
				if (!pRaid->locations[i])
					continue;

				PlayerClient* thisRaidMember = GetSpawnByName(pRaid->raidMembers[i].Name);
				if (!thisRaidMember)
					continue;

				char buffer[MAX_STRING] = { 0 };
				strcpy_s(buffer, thisRaidMember->Name);
				if (pRaid->raidMembers[i].RaidLeader)
					strcat_s(buffer, " (RL)");

				if (pRaid->raidMembers[i].RaidMainAssist)
					strcat_s(buffer, " (MA)");

				if (pRaid->raidMembers[i].GroupLeader) {
					char buffer2[12] = { 0 };
					sprintf_s(buffer2, " GL (%d)", pRaid->raidMembers[i].GroupNumber);
					strcat_s(buffer, buffer2);
				}

				if (pRaid->raidMembers[i].MasterLooter) {
					strcat_s(buffer, " (ML)");
				}


				ImGui::TableNextColumn();
				ImGui::Text(buffer);
				ImGui::Spacing();

				//health
				float healthpct = 0.0f;
				if (float maxhealth = static_cast<float>(thisRaidMember->HPMax))
					healthpct = thisRaidMember->HPCurrent / maxhealth;

				//endurance
				float endpct = 0.0f;
				if (float maxend = static_cast<float>(thisRaidMember->GetMaxEndurance())) {
					endpct = thisRaidMember->GetCurrentEndurance() / maxend;
				}

				//mana
				float manapct = 0.0f;
				if (float maxMana = static_cast<float>(thisRaidMember->GetMaxMana()))
					manapct = thisRaidMember->GetCurrentMana() / maxMana;

				static float barheight = 15.0f;
				//health, range
				ImGui::PushStyleColor(ImGuiCol_PlotHistogram, red_dark);
				ImGui::ProgressBar(healthpct, ImVec2(100, barheight));
				ImGui::PopStyleColor(1);
				//endurance, range
				ImGui::PushStyleColor(ImGuiCol_PlotHistogram, yellow_dark);
				ImGui::ProgressBar(endpct, ImVec2(100, barheight));
				ImGui::PopStyleColor(1);
				//mana, range
				ImGui::PushStyleColor(ImGuiCol_PlotHistogram, blue_dark);
				ImGui::ProgressBar(manapct, ImVec2(100, barheight));
				ImGui::PopStyleColor(1);
				ImGui::Spacing();

			}

			ImGui::EndTable();

			if (pRaid->Locked)
				ImGui::Text("Locked");

			ImGui::PopStyleColor(iPushPopColor);//This is popping ImGuiCol_Tab... stuff
			ImGui::PopStyleVar(iPushPopVar);
			ImGui::End();
		}
	}
}

/**
 * @fn OnMacroStart
 *
 * This is called each time a macro starts (ex: /mac somemacro.mac), prior to
 * launching the macro.
 *
 * @param Name const char* - The name of the macro that was launched
 */
PLUGIN_API void OnMacroStart(const char* Name)
{
	// DebugSpewAlways("MQ2RaidViewer::OnMacroStart(%s)", Name);
}

/**
 * @fn OnMacroStop
 *
 * This is called each time a macro stops (ex: /endmac), after the macro has ended.
 *
 * @param Name const char* - The name of the macro that was stopped.
 */
PLUGIN_API void OnMacroStop(const char* Name)
{
	// DebugSpewAlways("MQ2RaidViewer::OnMacroStop(%s)", Name);
}

/**
 * @fn OnLoadPlugin
 *
 * This is called each time a plugin is loaded (ex: /plugin someplugin), after the
 * plugin has been loaded and any associated -AutoExec.cfg file has been launched.
 * This means it will be executed after the plugin's @ref InitializePlugin callback.
 *
 * This is also called when THIS plugin is loaded, but initialization tasks should
 * still be done in @ref InitializePlugin.
 *
 * @param Name const char* - The name of the plugin that was loaded
 */
PLUGIN_API void OnLoadPlugin(const char* Name)
{
	// DebugSpewAlways("MQ2RaidViewer::OnLoadPlugin(%s)", Name);
}

/**
 * @fn OnUnloadPlugin
 *
 * This is called each time a plugin is unloaded (ex: /plugin someplugin unload),
 * just prior to the plugin unloading.  This means it will be executed prior to that
 * plugin's @ref ShutdownPlugin callback.
 *
 * This is also called when THIS plugin is unloaded, but shutdown tasks should still
 * be done in @ref ShutdownPlugin.
 *
 * @param Name const char* - The name of the plugin that is to be unloaded
 */
PLUGIN_API void OnUnloadPlugin(const char* Name)
{
	// DebugSpewAlways("MQ2RaidViewer::OnUnloadPlugin(%s)", Name);
}