using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HomeState
{
    home,
    deck,
    gatya,
    option
}
public class HomeUIController : MonoBehaviour
{
    [SerializeField]
    GameObject homePanel;
    [SerializeField]
    GameObject deckPanel;
    [SerializeField]
    GameObject gatyaPanel;
    [SerializeField]
    GameObject optionPanel;

    HomeState homeState;

    public void ActivePanelChenged(HomeState nextState)
    {
        switch (homeState)
        {
            case HomeState.home:
                homePanel.SetActive(false);
                break;
            case HomeState.deck:
                deckPanel.SetActive(false);
                break;
            case HomeState.gatya:
                gatyaPanel.SetActive(false);
                break;
            case HomeState.option:
                optionPanel.SetActive(false);
                break;
        }
        switch (nextState)
        {
            case HomeState.home:
                homePanel.SetActive(true);
                break;
            case HomeState.deck:
                deckPanel.SetActive(true);
                break;
            case HomeState.gatya:
                gatyaPanel.SetActive(true);
                break;
            case HomeState.option:
                optionPanel.SetActive(true);
                break;
        }
    }
}
