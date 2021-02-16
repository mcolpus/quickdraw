using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : MonoBehaviour {
    public int panelNumber;
    private CharacterSelectMenu characterSelectMenu;//this will contain useful stuff like list of random names
    public Player player;
    public bool open;
    public GameObject infoPanel;
    public InputField nameText;
    public Image characterTypeImage;
    public Image hatImage;
    public Text Description;
    public Text closeOpenText;

    public int characterTypeNumber;
    public int hatNumber;

    void Awake()
    {
        characterSelectMenu = GetComponentInParent<CharacterSelectMenu>();
    }

    void Start()
    {
        PressRandomizeName();
        PressNextCharacterType(0);
        PressNextHatType(0);
        infoPanel.SetActive(false);
        closeOpenText.text = "Enter";
    }

    public Player GetPlayerData()
    {
        //called by start menu when game is about to begin
        return player;
    }

    public void UpdateData(CharacterSelectMenu.PanelData data)
    {
        open = data.open;

        if(open)
        {
            infoPanel.SetActive(true);
            closeOpenText.text = "Exit";
        }
        else
        {
            infoPanel.SetActive(false);
            closeOpenText.text = "Enter";
        }

        nameText.text = data.name;
        characterTypeNumber = data.characterOptionNumber;
        CharacterSelectMenu.CharacterOption option = characterSelectMenu.GetCharacterOption(characterTypeNumber);
        player.characterType = option.type;
        player.spriteColor = option.spriteColor;
        Description.text = option.description;
        characterTypeImage.sprite = option.sprite;
        characterTypeImage.color = option.spriteColor;


        hatNumber = data.hatOptionNumber;
        CharacterSelectMenu.HatOption hatOption = characterSelectMenu.GetHatOption(hatNumber);
        player.hat = hatOption.sprite;
        if(hatOption.sprite != null)
        {
            hatImage.color = Color.white;
            hatImage.sprite = hatOption.sprite;
        }
        else
        {
            hatImage.color = Color.clear;
        }
    }

    //these methods are called by the UI elements
    public void PressRandomizeName()
    {
        characterSelectMenu.RandomizeName(panelNumber);
    }
    public void PressOpenClose()
    {
        characterSelectMenu.OpenClose(panelNumber);
    }
    public void PressNextCharacterType(int amount)
    {
        characterSelectMenu.NextCharacterType(panelNumber, amount);
    }
    public void PressNextHatType(int amount)
    {
        characterSelectMenu.NextHatType(panelNumber, amount);
    }
    public void OnEditName(string _name)
    {
        if(characterSelectMenu!=null)
            characterSelectMenu.SetName(panelNumber, nameText.text);
    }

    //public void RandomizeName()
    //{
    //    string rnd = characterSelectMenu.GetRandomName(player.name);
    //    player.name = rnd;
    //    nameText.text = "Name: " + rnd;
    //}

    //public void OpenClose()
    //{
    //    if(open)
    //    {
    //        Close();
    //    }
    //    else
    //    {
    //        Open();
    //    }
    //}

    //private void Close()
    //{
    //    open = false;
    //    infoPanel.SetActive(false);
    //    closeOpenText.text = "Enter";
    //}
    //private void Open()
    //{
    //    open = true;
    //    infoPanel.SetActive(true);
    //    closeOpenText.text = "Exit";
    //}

    //public void NextCharacterType(int amount)
    //{
    //    characterTypeNumber += amount;
    //    CharacterSelectMenu.CharacterOption option = characterSelectMenu.GetCharacterOption(characterTypeNumber);
    //    player.characterType = option.type;
    //    player.spriteColor = option.spriteColor;
    //    Description.text = option.description;
    //    characterTypeImage.sprite = option.sprite;
    //    characterTypeImage.color = option.spriteColor;
    //}

    //public void NextHat(int amount)
    //{
    //    hatNumber += amount;
    //    CharacterSelectMenu.HatOption option = characterSelectMenu.GetHatOption(hatNumber);
    //    player.hat = option.sprite;
    //    if(option.sprite != null)
    //    {
    //        hatImage.color = Color.white;
    //        hatImage.sprite = option.sprite;
    //    }
    //    else
    //    {
    //        hatImage.color = Color.clear;
    //    }
    //}
	
}
