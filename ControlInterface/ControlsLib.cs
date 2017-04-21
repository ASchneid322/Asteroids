//***********************************************************************************
//Program:					ControllInterface
//Description:	            Gets controls from the keyboard or a xontroller
//                          and assigns them to a public interface
//Date:						Feb 17th 2017
//Authors:					Alexander Schneider
//Course:					CMPE2800
//Class:					CNTA02
//***********************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ControlsLib
{
    public class ControlInterface
    {
        //Public interface for classes to read state of inputs.
        public bool up { get; private set; }
        public bool down { get; private set; }
        public bool left { get; private set; }
        public bool right { get; private set; }
        public bool fire { get; private set; }
        public bool pause { get; private set; }

        //Controls mapped to keys
        private int upKey = 'W';
        private int downKey = 'S';
        private int leftKey = 'A';
        private int rightKey = 'D';
        private int fireKey = ' ';
        private int pauseKey = 'P';

        //Checks if the controller is connected
        private bool controllerConnected;
        //Thread for controller polling
        private Thread pollThread;
        //Time the thread sleeps.
        private readonly int waitTime;

        /// //////////////
        /// Contructor sets the wait time based on FPS
        /// Starts thread to poll controller inputs.
        /// //////////////
        public ControlInterface(int frameRate = 60)
        {
            //Wait time is found by ideal game FPS
            waitTime = 1000 / frameRate;
            //Start the polling thread.
            pollThread = new Thread(pollController);
            pollThread.Start();
        }
        /// ////////////
        /// Updates keys from the keyboard events
        /// sent in from the main fourm.
        /// Accept KeyEventArgs for a new keypress
        /// Bool keyDown for if its a keydown or up to unlatch
        /// ///////////
        public void updateKeys(KeyEventArgs newKey, bool keyDown)
        {
            //If the controller is connected no keyboard inputs
            if (!controllerConnected)
            {
                if (keyDown)
                {
                    //Up and down together so you can't do both
                    if (newKey.KeyValue == upKey)
                        up = true;
                    else if (newKey.KeyValue == downKey)
                        down = true;
                    //Left and right together so you can't do both
                    if (newKey.KeyValue == leftKey)
                        left = true;
                    else if (newKey.KeyValue == rightKey)
                        right = true;
                    if (newKey.KeyValue == fireKey)
                        fire = true;
                    if (newKey.KeyValue == pauseKey)
                        pause = true;
                }
                //If the key is released
                else if (!keyDown)
                {
                    if (newKey.KeyValue == upKey)
                        up = false;
                    else if (newKey.KeyValue == downKey)
                        down = false;
                    if (newKey.KeyValue == leftKey)
                        left = false;
                    else if (newKey.KeyValue == rightKey)
                        right = false;
                    if (newKey.KeyValue == fireKey)
                        fire = false;
                    if (newKey.KeyValue == pauseKey)
                        pause = false;
                }
            }
        }

        /// ///////////
        /// Function to poll the controller for its controls
        /// ///////////
        private void pollController()
        {
            GamePadState gps;
            //Thread runs forever
            while (true)
            {
                //Checks if the controller is connected
                gps = GamePad.GetState(PlayerIndex.One);
                controllerConnected = gps.IsConnected;
                if (controllerConnected)
                {
                    //Set the controls when they are pressed
                    //Up and down bound together
                    if (gps.IsButtonDown(Buttons.DPadUp))
                        up = true;
                    else if (gps.IsButtonDown(Buttons.DPadDown))
                        down = true;
                    else
                    {
                        down = false;
                        up = false;
                    }
                    //Left and right bound together
                    if (gps.IsButtonDown(Buttons.DPadLeft))
                        left = true;
                    else if (gps.IsButtonDown(Buttons.DPadRight))
                        right = true;
                    else
                    {
                        left = false;
                        right = false;
                    }

                    //A to fire
                    if (gps.IsButtonDown(Buttons.A))
                        fire = true;
                    else
                        fire = false;

                    //Start to pause
                    if (gps.IsButtonDown(Buttons.Start))
                        pause = true;
                    else
                        pause = false;
                }
                //Sleep before checking for more inputs
                Thread.Sleep(waitTime);
            }
        }
    }
}