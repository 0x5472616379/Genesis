﻿using Genesis.Cache;
using Genesis.Configuration;
using Genesis.Constants;
using Genesis.Entities;
using Genesis.Environment;
using Genesis.Managers;
using Genesis.Model;

namespace Genesis;

public class PacketBuilder
{
    private readonly Player _player;

    public PacketBuilder(Player player)
    {
        _player = player;
    }

    public void SendPlayerStatus()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
        _player.Session.Writer.WriteByteA(0);
        _player.Session.Writer.WriteWordBigEndianA(_player.Session.Index);
    }

    public void SendNewBuildAreaPacket()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.REGION_LOAD);
        _player.Session.Writer.WriteWordA(_player.Location.CachedCenterChunkX);
        _player.Session.Writer.WriteWord(_player.Location.CachedCenterChunkY);
    }

    public void DisplayWelcomeScreen()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_WELCOME);
        _player.Session.Writer.WriteByteC(201); //daysSinceRecoveryChange
        _player.Session.Writer.WriteWordA(1); //unreadMessages
        _player.Session.Writer.WriteByte(0); //membership
        _player.Session.Writer.WriteDWordV2(IPAddressConverter.ConvertToInt("127.0.0.1")); //lastAddress
        _player.Session.Writer.WriteWordA(128); //daysSinceLogin
    }

    public void SendMessage(string message)
    {
        _player.Session.Writer.CreateFrameVarSize(ServerOpCodes.MSG_SEND);
        _player.Session.Writer.WriteString($"{message}");
        _player.Session.Writer.EndFrameVarSize();
    }

    public void SendSidebarInterface(int _tabId, int _displayId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        _player.Session.Writer.WriteWord(_displayId); /* What to display inside that tab */
        _player.Session.Writer.WriteByteA(_tabId); /* Which tab */
    }

    public void SendFriendListStatus(FriendListStatus _status)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.FRIENDLIST_STATUS);
        _player.Session.Writer.WriteByte((byte)_status); /* 0 is loading, 1 is connecting, 2 is loaded. */
    }

    public void SendConfig(int _id, int _state)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.CONFIG_SET);
        _player.Session.Writer.WriteWordBigEndian(_id);
        _player.Session.Writer.WriteByte(_state);
    }

    public void UpdateSlot(int slot, int itemId, int amount, int containerId)
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);

        _player.Session.Writer.WriteWord(containerId);

        _player.Session.Writer.WriteSmartB(slot);

        _player.Session.Writer.WriteWord(itemId + 1);

        if (amount > 254)
        {
            _player.Session.Writer.WriteByte(255);
            _player.Session.Writer.WriteDWord(amount);
        }
        else
        {
            _player.Session.Writer.WriteByte(amount);
        }

        _player.Session.Writer.EndFrameVarSizeWord();
    }


    public void SendTextToInterface(string _text, int _interfaceId)
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.INTF_TEXT_ADD);
        _player.Session.Writer.WriteString(_text);
        _player.Session.Writer.WriteWordA(_interfaceId);
        _player.Session.Writer.EndFrameVarSizeWord();
    }

    public void SendChatInterface(int interfaceId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_CHAT_ADD);
        _player.Session.Writer.WriteWordBigEndian(interfaceId);
    }

    public void DisplayHiddenInterface(int _mainFrame, int _frameId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_HIDDEN);
        _player.Session.Writer.WriteByte(_mainFrame); /* What to display inside that tab */
        _player.Session.Writer.WriteWord(_frameId); /* Which tab */
    }

    public void SendItemToInterface(int itemId, int zoom, int _interfaceId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_MODEL_ZOOM);
        _player.Session.Writer.WriteWordBigEndian(_interfaceId);
        _player.Session.Writer.WriteWord(zoom);
        _player.Session.Writer.WriteWord(itemId);
    }

    public void SendInterfaceOffset(int a, int b, int barId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_OFFSET);
        _player.Session.Writer.WriteWord(a);
        _player.Session.Writer.WriteWordBigEndian(b);
        _player.Session.Writer.WriteWordBigEndian(barId);
    }

    public void RefreshContainer(List<Item> container, int containerId, int count)
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SET);
        _player.Session.Writer.WriteWord(containerId);
        _player.Session.Writer.WriteWord(count);

        for (int i = 0; i < count; i++)
        {
            if (container[i] == null) continue;

            var amount = container[i].Amount;
            var itemId = container[i].Id;
            if (amount > 254)
            {
                _player.Session.Writer.WriteByte(255);
                _player.Session.Writer.WriteDWordV2(amount);
            }
            else
            {
                _player.Session.Writer.WriteByte(amount);
            }

            if (amount < 1) itemId = 0;
            if (itemId > ServerConfig.ITEM_LIMIT || itemId < 0) itemId = ServerConfig.ITEM_LIMIT;
            _player.Session.Writer.WriteWordBigEndianA(itemId <= 0 ? itemId : itemId + 1);
        }

        _player.Session.Writer.EndFrameVarSizeWord();
    }

    public void ShowInterface(int i)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_SHOW);
        _player.Session.Writer.WriteWord(i);
    }

    public void ClearAllInterfaces()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_CLEAR);
    }

    public void SendInterface(int interfaceId, int inventoryId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_INV_HUD);
        _player.Session.Writer.WriteWordA(interfaceId);
        _player.Session.Writer.WriteWord(inventoryId);
    }

    public void SendInteractionOption(string option, int slot, bool top)
    {
        _player.Session.Writer.CreateFrameVarSize(ServerOpCodes.PLAYER_RIGHTCLICK);
        _player.Session.Writer.WriteByteC(slot);
        _player.Session.Writer.WriteByteA(top ? 1 : 0);
        _player.Session.Writer.WriteString(option);
        _player.Session.Writer.EndFrameVarSize();
    }

    public void SendSkillUpdate(int skillId, int exp, int boostedLevel)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.PLAYER_SKILL);
        _player.Session.Writer.WriteByte(skillId);
        _player.Session.Writer.WriteDWordV1(exp);
        _player.Session.Writer.WriteByte(boostedLevel);
    }

    public void SendSound(int trackId, int loop, int delay)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.SOUND_PLAY);
        _player.Session.Writer.WriteWord(trackId);
        _player.Session.Writer.WriteByte(loop);
        _player.Session.Writer.WriteWord(delay);
    }

    public void SendSong(int trackId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.SONG_PLAY);
        _player.Session.Writer.WriteWordBigEndian(trackId);
    }

    /// <summary>
    /// Sends the SW X/Y of the Chunk that we will send edits to
    /// </summary>
    /// <param name="location"></param>
    public void SendActiveChunk(int x, int y)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.ACTIVE_CHUNK);
        _player.Session.Writer.WriteByteC(y);
        _player.Session.Writer.WriteByteC(x);
    }

    public void UpdateObject(int x, int y, WorldObject worldObject)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.OBJ_ADD);
        _player.Session.Writer.WriteByteA(((x & 0x7) << 4) | (y & 0x7));
        _player.Session.Writer.WriteWordBigEndian(worldObject.Id);
        _player.Session.Writer.WriteByteS((worldObject.Type << 2) | (worldObject.Direction & 3));
    }
    
    public void UpdateObject(int x, int y, ModifiedEntity worldObject)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.OBJ_ADD);
        _player.Session.Writer.WriteByteA(((x & 0x7) << 4) | (y & 0x7));
        _player.Session.Writer.WriteWordBigEndian(worldObject.Id);
        _player.Session.Writer.WriteByteS((worldObject.Type << 2) | (worldObject.Face & 3));
    }
}