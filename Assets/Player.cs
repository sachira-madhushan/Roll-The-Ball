using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using TMPro;
public class Player : MonoBehaviour
{
    SerialPort serialPort;
    float xVelocity, yVelocity;
    private Rigidbody rb;
    Vector3 playerTransform;
    bool gameStarted=false;
    public GameObject Canvas;
    public TextMeshProUGUI motivation;
    float timer = 0;
    bool timerStarted = false;
    public GameObject Button;
    void Start()
    {
        serialPort = new SerialPort("COM4", 115200);

        serialPort.Open();
        Task.Run(() => SerialThread());
        rb = GetComponent<Rigidbody>();
        playerTransform = this.transform.position;
        print(playerTransform);
        Invoke("StartGame", 5);
        Canvas.SetActive(true);
        Button.SetActive(false);
    }

    public void StartGame()
    {
        Canvas.SetActive(false);
        gameStarted = true;
    }
    async void SerialThread()
    {
        while (true)
        {
            SerialRead();
        }
    }
    void SerialRead()
    {
        if (serialPort.IsOpen)
        {
            string value = serialPort.ReadLine();
            xVelocity = float.Parse(value.Split(" ")[0]);
            yVelocity = float.Parse(value.Split(" ")[1]);


        }

    }
    void Update()
    {
        if (timerStarted)
        {
            rb.velocity = new Vector3(0, -20, 0);
            timer += Time.deltaTime;
            if (timer > 5)
            {
                
                Canvas.SetActive(false);
                gameStarted = true;
                timerStarted = false;
                timer = 0;
            }
        }
        if (!gameStarted) return;
        
        if (this.transform.position.y < -20)
        {
            rb.velocity = new Vector3(0, -20, 0);
            this.transform.position = playerTransform;
            Canvas.SetActive(true);
            motivation.text = "Do not judge me by my successes, judge me by how many times I fell down and got back up again!";
            timerStarted = true;
            gameStarted = false;
            

        }
        if (!(xVelocity > 2 || xVelocity > -2 && yVelocity > 2 || yVelocity < 2))
        {
            rb.velocity = new Vector3(0,-20, 0);
            
        }
        rb.velocity = new Vector3(yVelocity*Time.timeScale, -20, -xVelocity*Time.timeScale);

        

    }
    private void OnApplicationQuit()
    {
        serialPort.Close();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            Canvas.SetActive(true);
            motivation.text = "I will win Level 2024!\nI will face every challenge, navigate every obstacle, and give my best to conquer each level of life. Success is the result of relentless effort, and I am committed to winning my own game.";
            gameStarted = false;
            rb.velocity = new Vector3(0, -20, 0);
            Button.SetActive(true);
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
