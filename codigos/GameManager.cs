using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    
    
    /* public int m_NumRoundsToWin = ;     */
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;
    public Text playerNameText;

    public float timer = 600;

    public Text TextoTimer;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;

    private float currentTimeInSeconds;//prueba

    private void UpdateTimerText()
    {
        // Convertir el tiempo actual en minutos y segundos
        int minutes = Mathf.FloorToInt(currentTimeInSeconds / 60);
        int seconds = Mathf.FloorToInt(currentTimeInSeconds % 60);

        // Mostrar el temporizador en el formato MM:SS
        TextoTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void Update()
    {

        /* timer -= Time.deltaTime;
         TextoTimer.text = "" + timer.ToString("f0");
         if (timer <= 0)
         {
             timer = 0;
         }*/
        if (currentTimeInSeconds > 0)
        {
            // Reducir el tiempo restante en cada frame solo si es mayor que cero
            currentTimeInSeconds -= Time.deltaTime;

            // Actualizar el texto del temporizador
            UpdateTimerText();

            // Si el tiempo restante llega a cero, llamar a la función GameOver
            if (currentTimeInSeconds <= 0)
            {
                currentTimeInSeconds = 0; // Asegurar que el tiempo restante sea cero
                GameOver();
            }

            
        }
    }
    private void Start()
    {

        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
        playerNameText.text = "Orlin - 1-16-8387";
        currentTimeInSeconds = timer;// ajusto tiempo total de timer al actual

    }

    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }

    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {

        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        
        else
        {
            StartCoroutine(GameLoop());
        }
                
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        m_MessageText.text = string.Empty;
        
         while (!OneTankLeft())
         {
             yield return null;  

         }

    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;
        m_GameWinner = GetGameWinner();
        string message = EndMessage();
        m_MessageText.text = message;


        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
       /* for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins >= 2)
            {
                return true; 
            }
        }*/
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
                
        
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        /*for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin) original
                return m_Tanks[i];
        }*/
        //aqui modifico para  poner el numero de wins
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == 10)
                return m_Tanks[i];
        }

        return null;
    }


    public string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private void GameOver()
    {
        // Mostrar el mensaje de "Game Over"
        m_MessageText.text = "Game Over Ambos perdieron ";

        if (currentTimeInSeconds <= 0)
        {
            SceneManager.LoadScene(1);
        }

        else
        {
            StartCoroutine(GameLoop());
        }


    }
}