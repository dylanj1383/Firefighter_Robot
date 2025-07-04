'Chip
#Chip 16f18875, 32

'-------------------------CONSTANTS'-------------------------
'Motor Pins
#Define Motor_LB_Forward PortA.0
#Define Motor_LB_Backward PortA.1
#Define Motor_RB_Forward PortA.3
#Define Motor_RB_Backward PortA.2
#Define Motor_LF_Forward PortE.1
#Define Motor_LF_Backward PortE.2
#Define Motor_RF_Forward PortA.5
#Define Motor_RF_Backward PortE.0

'LCD pins
#Define LCD_LINES 2
#Define LCD_WIDTH 16
#Define LCD_IO 4
#Define LCD_DB4 PortD.4
#Define LCD_DB5 PortD.5
#Define LCD_DB6 PortD.6
#Define LCD_DB7 PortD.7
#Define LCD_RS PortD.0
#Define LCD_RW PortD.1
#Define LCD_ENABLE PortD.2
#Define LCD_SPEEDFAST

'Fan Pin
#Define Fan PortB.7

'Sensor pins/Setup

'Flame 1 (Front)
#Define Flame_Sensor_1 ANC2 'analog
#Define Flame_Threshold 100 'sensitivity for flame
#Define Flame_Detect_1 PortC.2 'port

'Flame 2 
#Define Flame_Sensor_2 ANB5 'analog
#Define Flame_Detect_2 PortB.6 'port

'Flame 3
#Define Flame_Sensor_3 ANB6 'analog
#Define Flame_Detect_3 PortB.56 'port

'Line
#Define Line_Detect PortC.3 'port
INLVLC3 = 0 'TTl input voltage levels for RC3

'Wall
#Define Wall_Left_Detect PortC.0
#Define Wall_Left_Sensor ANC0

#Define Wall_Front_Detect PortC.1
#Define Wall_Front_Sensor ANC1


'-------------------------Input/Output-------------------------

'Set Outputs 
Dir Motor_LB_Forward Out
Dir Motor_LB_Backward Out
Dir Motor_RB_Forward Out
Dir Motor_RB_Backward Out
Dir Motor_LF_Forward Out
Dir Motor_LF_Backward Out
Dir Motor_RF_Forward Out
Dir Motor_RF_Backward Out

Dir Fan Out

Dir LCD_RS Out
Dir LCD_RW Out
Dir LCD_ENABLE Out
Dir LCD_DB4 Out
Dir LCD_DB5 Out
Dir LCD_DB6 Out
Dir LCD_DB7 Out

'Set Inputs
Dir Flame_Detect_1 In
Dir Flame_Detect_2 In
Dir Flame_Detect_3 In
Dir Line_Detect In 
Dir Wall_Left_Detect In
Dir Wall_Front_Detect In

'-------------------------VARIABLES-------------------------

Dim Flame_Value_1 as Byte 'value for flame level
Dim Flame_Value_2 as Byte
Dim Flame_Value_3 as Byte

Dim Wall_Left_Value as Byte 
Dim Wall_Front_Value as Byte
Dim Wall_Left_Distance as Byte
Dim Wall_Front_Distance as Byte

Dim Immune as Long
Immune = 0

Dim Saw_Black as Byte
Saw_Black = 0

Dim Line_Count as Long ' used only for maze 2
Line_Count = 0

DIM MAZE as Byte 'which maze we are solving

'-------------------------SET INITIAL PINOUT-------------------------
Motor_RB_Forward = 0
Motor_RB_Backward = 0
Motor_LB_Forward = 0
Motor_LB_Backward = 0
Motor_RF_Forward = 0
Motor_RF_Backward = 0
Motor_LF_Forward = 0
Motor_LF_Backward = 0

Fan = 0

'-------------------------TESTING SUBROUTINES-------------------------
Sub Test_Motor_Loop ()
    ''to test if the motor hardware is correct
    Move_Forward
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms

    Move_Backward
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms

    Move_Right
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms

    Move_Left
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms

    Turn_Clockwise
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms

    Turn_Anti_Clockwise
    Wait 4000 ms

    Move_Stop
    Wait 2000 ms
End Sub

Sub Test_Fan_Loop ()
    ' to test if the fan hardware is correct
    Fan_On 'there is a delay for sending the signal and the fan actually turning on
    Wait 6000 ms 

    Fan_Off 'there is no delay for cutting off the signal and the fan actually turning off
    Wait 3000 ms 
End Sub

Sub Wall_Detect_With_LCD_Loop()
    ''to test if wall sensor hardward is correct (and LCD)
    Wall_Left_Value = ReadAD (Wall_Left_Sensor)
    Wall_Front_Value = ReadAD (Wall_Front_Sensor)

    Wall_Left_Distance = ((6787 / (Wall_Left_Value - 3))-4) / 5
    Wall_Front_Distance = ((6787 / (Wall_Front_Value - 3))-4) / 5

    CLS
    Locate 0,0
    Print "Front: "
    Print Wall_Front_Distance
    Locate 1,0
    Print "Left: "
    Print Wall_Left_Distance
End Sub

Sub Flame_Detect_With_LCD_Loop()
    ''to test if flame detect hardware is correct (and LCD)
    ''also used for finding the ideal thresholds as we scan for the room 4 runs in both mazes
    Flame_Value_1 = ReadAD( Flame_Sensor_1)
    Flame_Value_2 = ReadAD( Flame_Sensor_2)
    Flame_Value_3 = ReadAD( Flame_Sensor_3)

    CLS
    Locate 0,0
    Print "F1: "
    Print Flame_Value_1
    Locate 0,8
    Print "F2: "
    Print Flame_Value_2
    Locate 1,0
    Print "F3: "
    Print Flame_Value_3
End Sub

'-------------------------GENERAL MAZE SOLVING SUBROUTINES-------------------------
Sub Move_Forward ()
    Motor_RB_Forward = 1
    Motor_RB_Backward = 0
    Motor_LB_Forward = 1
    Motor_LB_Backward = 0
    Motor_RF_Forward = 1
    Motor_RF_Backward = 0
    Motor_LF_Forward = 1
    Motor_LF_Backward = 0
End Sub

Sub Move_Backward ()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 1
    Motor_LB_Forward = 0
    Motor_LB_Backward = 1
    Motor_RF_Forward = 0
    Motor_RF_Backward = 1
    Motor_LF_Forward = 0
    Motor_LF_Backward = 1
End Sub

Sub Move_Right ()
    Motor_RB_Forward = 1
    Motor_RB_Backward = 0
    Motor_LB_Forward = 0
    Motor_LB_Backward = 1
    Motor_RF_Forward = 0
    Motor_RF_Backward = 1
    Motor_LF_Forward = 1
    Motor_LF_Backward = 0
End Sub

Sub Move_Left ()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 1
    Motor_LB_Forward = 1
    Motor_LB_Backward = 0
    Motor_RF_Forward = 1
    Motor_RF_Backward = 0
    Motor_LF_Forward = 0
    Motor_LF_Backward = 1
End Sub

Sub Move_Stop ()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 0
    Motor_LB_Forward = 0
    Motor_LB_Backward = 0
    Motor_RF_Forward = 0
    Motor_RF_Backward = 0
    Motor_LF_Forward = 0
    Motor_LF_Backward = 0
End Sub

Sub Turn_Clockwise ()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 1
    Motor_LB_Forward = 1
    Motor_LB_Backward = 0
    Motor_RF_Forward = 0
    Motor_RF_Backward = 1
    Motor_LF_Forward = 1
    Motor_LF_Backward = 0
End Sub

Sub Turn_Anti_Clockwise ()
    Motor_RB_Forward = 1
    Motor_RB_Backward = 0
    Motor_LB_Forward = 0
    Motor_LB_Backward = 1
    Motor_RF_Forward = 1
    Motor_RF_Backward = 0
    Motor_LF_Forward = 0
    Motor_LF_Backward = 1
End Sub

Sub Move_Front_Right()
    Motor_RB_Forward = 1
    Motor_RB_Backward = 0
    Motor_LB_Forward = 0
    Motor_LB_Backward = 0
    Motor_RF_Forward = 0
    Motor_RF_Backward = 0
    Motor_LF_Forward = 1
    Motor_LF_Backward = 0
End Sub

Sub Move_Front_Left()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 0
    Motor_LB_Forward = 1
    Motor_LB_Backward = 0
    Motor_RF_Forward = 1
    Motor_RF_Backward = 0
    Motor_LF_Forward = 0
    Motor_LF_Backward = 0
End Sub

Sub Move_Back_Right()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 0
    Motor_LB_Forward = 0
    Motor_LB_Backward = 1
    Motor_RF_Forward = 0
    Motor_RF_Backward = 1
    Motor_LF_Forward = 0
    Motor_LF_Backward = 0
End Sub

Sub Move_Back_Left()
    Motor_RB_Forward = 0
    Motor_RB_Backward = 1
    Motor_LB_Forward = 0
    Motor_LB_Backward = 0
    Motor_RF_Forward = 0
    Motor_RF_Backward = 0
    Motor_LF_Forward = 0
    Motor_LF_Backward = 1
End Sub

Sub Fan_On ()

    'because our blower, even though its rlly powerful,  is not meant for embedded systems and uses its own 
    'circuitry for powering brushless DC motors with lithium ion rechargeable batteries.
    'we just replaced the builtin push switch with a transistor - so we can send a signal with the pic
    'to simulate holding down the push switch
    'however, there is a 1 s delay between when u hold down the button and the fan turns on, 
    'so there is a 1 second delay between when this subroutine is called and the fan actually turns on
    Fan = 1
End Sub

Sub Fan_Off ()
    Fan = 0
End Sub

Sub Turn_Right_90()
    Turn_Clockwise
    Wait 270 ms 
    Turn_Anti_Clockwise
    Wait 80 ms
    Move_Stop
End Sub    

Sub Turn_Left_90()
    Turn_Anti_Clockwise

    ''depending on the maze, the time we turn for varies slightly
    If MAZE = 1 Then 
        Wait 280 ms 
    Else If MAZE = 2 tHEN 
        Wait 295 ms 
    End If 
    'break to cancel out the angular momentum it has
    Turn_Clockwise
    Wait 80 ms
    Move_Stop
End Sub    

Sub Adjust_Right()
    ' for some reason the optimal amount we adjust is depenedent on the maze
    If MAZE = 1 Then 
        Turn_Clockwise
        Wait 400 ms
    Else If MAZE = 2 Then  
        Turn_Clockwise
        Wait 25 us
    End If
End Sub

Sub Adjust_Left()
    If MAZE = 1 Then 
        Turn_Anti_Clockwise
        Wait 400 us 'us = microseconds
    Else If MAZE = 2 Then 
        Turn_Anti_Clockwise
        Wait 25 us 
    End If 
End Sub

Sub Read_Sensors()
    ''reads all our sensor values and updates them to the global variables
    Wall_Left_Value = ReadAD (Wall_Left_Sensor)
    Wall_Front_Value = ReadAD (Wall_Front_Sensor)

    Wall_Left_Distance = ((6787 / (Wall_Left_Value - 3))-4) / 5
    Wall_Front_Distance = ((6787 / (Wall_Front_Value - 3))-4) / 5

    Flame_Value_1 = ReadAD( Flame_Sensor_1)
    Flame_Value_2 = ReadAD( Flame_Sensor_2)
    Flame_Value_3 = ReadAD( Flame_Sensor_3)
End Sub 

'-------------------------MAZE 1 SPECIFIC SUBROUTINES-------------------------
Sub Extinguish()
    'Extinguishes a flame, assuming the bot has stopped within 30 of the flame
    Move_Stop
    Do Forever
        Read_Sensors

        If Flame_Value_1 > 200 Then
            Fan_Off
        Else
            'Turn fan on and wait. Then turn a bit and wait. Then turn the other way and wait. 
            'Repeat this until extinguished
            Fan_On
            Wait 5 s

            Turn_Anti_Clockwise
            Wait 100 ms 

            Move_Stop
            Wait 5 s 

            Turn_Clockwise
            Wait 200 ms 

            Move_Stop
            Wait 5 s

            Turn_Anti_Clockwise
            Wait 100 ms 

            Move_Stop
            Wait 5 s
        End If
    Loop
End Sub

Sub Straight_Left_Wall_Follow()
    'Follows a straight left wall; doesn't worry about concave/convex corners
    Read_Sensors

    If Wall_Left_Distance < 10 Then
        Adjust_Right
        Move_Front_Right
    
    Else If Wall_Left_Distance > 16 Then
        Adjust_Left
        Move_Front_Left
    Else
        Move_Forward
    End If
End Sub

Sub Flame_Near() 
    'called when a flame is near. That is, we are pointing at a flame, but don't know how far we are from it
    Do Forever
        Read_Sensors

        If (28 < Wall_Front_Distance < 35) and (Immune = 0) Then
            ''we are immune if we have just turned around a left wall (i.e. the lwall > 30 case in left wall follow)
            ''we become unimmune if we can once again see a wall to our left

            ''we can only go to extinguish if we are unimmune

            'Break for a hard stop (we are within 30 of a flame and want to stop immediately)
            Move_Backward
            Wait 100 ms
            Move_Stop
            Extinguish
        End If

        'If flame near but not within 30, continue straight left wall follow
        Straight_Left_Wall_Follow
    Loop
End Sub

Sub Check_Flame()
    'checks the flame value and runs flame_near if appropriate
    Read_Sensors

    If (Flame_Value_1 < 8) and (Immune = 0)Then
        Flame_Near
    End If
End Sub

Sub M1_Left_Wall_Follow()
    'decrement immune every frame. Immune will be set to the number of 'frames' to be immune for
    'and we are not immune when it gets to 0
    If Immune > 0 Then
        Immune = Immune - 1
    End If

    Read_Sensors

    Check_Flame

    'Nothing to our left. We want to turn left. 
    If Wall_Left_Distance > 30 Then
        Immune = 100 'make sure we don't get messed up by a flame as we are in the process of turning around a corner

        'Inch forward so we don't ram into the wall we are turning around
        Move_Forward
        Wait 130 ms
        Move_Backward 'break for a hard stop
        Wait 60 ms

        'Stop briefly to reset ourselves. The turn 90s are assuming the bot has no momentum currently
        Move_Stop
        Wait 300 ms

        Turn_Left_90
        Move_Stop
        Wait 300 ms

        'Check flame here - we may have just turned a corner and are now facing a flame
        Check_Flame

        'If not, move forward so that we are passed the corner. 
        Move_Forward
        Wait 490 ms
        Move_Backward 'break for a hard stop
        Wait 60 ms

        'Once again, check_flame. We may now be facing a flame. 
        Move_Stop
        Check_Flame

    'Somethign to our left and something in front of us - we want to turn right
    Else If Wall_Front_Distance < 29 Then    
        Immune = 10 'Set ourselves to immune, but not as long as when we turn left (turning right is a much shorter process)

        'break for a hard stop to avaoid ramming into a wall 
        Move_Backward 
        Wait 80 ms

        'stop and check flame
        Move_Stop 
        Check_Flame


        'Turn right 90 from a dead start
        Wait 300 ms
        Turn_Right_90

        'stop and check flame
        Move_Stop
        Wait 100 ms
        Check_Flame
    
    ''If none of the above cases, we are just following a striaght left wall
    'adjust periodically to maintain our distance and alignment with the wall


    Else If Wall_Left_Distance < 10 Then
        'too close - adjust right (turn ourselves a bit away from the wall so we realign)
        'and also strafe front-right so we dont continuously adjust right
        Adjust_Right
        Move_Front_Right
    
    Else If Wall_Left_Distance > 16 Then
        ''too far - adjust left (turn ourselves a bit closer to the wall so we realign)
        'and also strafe front-left so we don't continuously adjust left
        Adjust_Left
        Move_Front_Left

    ''If none of the above, we are just optimal distance from wall - continue moving forward
    Else
        Move_Forward
    End If
End Sub

Sub Setup_Wall_Follow()
    'If we were about to go to room 4 but saw a flame in rooms 1/2/3, set up for wall following
    Fan_Off

    Move_Back_Left
    Wait 280 ms

    Turn_Clockwise
    Wait 300 ms

    Move_Forward
    Wait 520 ms

    Move_Stop
    Wait 30 ms

    Immune = 100
End Sub

Sub Fast_Room_4()
    ''The inital code that runs at the start of maze 1
    ''We go to the middle so that our flame sensor is just past the wall and scan all 3 rooms in place
    ''If there is no flame int he other 3 rooms and there is one in room 4, head straight for room 4
    ''Otherwise, setup for left wall following and solve the other 3 rooms

    ''because of the way our fan works (See fan on for more explanation), we need to send the signal now 
    ''and if there isn't a flame in room 4, just turn off the signal and the fan wont turn on at all
    Fan_On

    'move to the middle of the maze, adjust clockwise to account for bot's natural drift
    Move_Left
    Wait 300 ms
    Turn_Clockwise
    Wait 20 ms

    Move_Left
    Wait 300 ms 
    Turn_Clockwise 
    Wait 10 ms 
    
    Move_Left
    Wait 520 ms

    ''now at the intersection. Turn 45 so our sensors can scan

    Turn_Clockwise
    Wait 180 ms 'adjust so we turn 45 degrees
    Turn_Anti_Clockwise
    Wait 55 ms 'break
   
    Read_Sensors
    
    ''Print flame values for tuning the thresholds
    CLS
    Locate 0,0
    Print "F1: "
    Print Flame_Value_1
    Locate 0,8
    Print "F2: "
    Print Flame_Value_2
    Locate 1,0
    Print "F3: "
    Print Flame_Value_3


    ''If flame in any of the other three rooms, setup wall follow and wall follow
    If Flame_Value_2 < 220 or Flame_value_3 < 220 Then
        Setup_Wall_Follow 'this includes killing the fan-on signal
    
    ''Else, go straight to room 4 and extinguish
    Else
        Move_Front_Right
        Wait 20 ms 
        Move_Forward
        Wait 930 ms
        Move_Stop
        Wait 100 s
    End If
End Sub

'-------------------------MAZE 2 SPECIFIC SUBROUTINES-------------------------
Sub Check_Line()
    'Check to detect the leading edge of any lines we cross

    'if on black, set saw_black to true
    If Line_Detect = 1 Then
        Saw_Black = 1

    'if currently on white and previously on black, increment line count
    Else If (Line_Detect = 0) and (Saw_Black = 1) Then
        Saw_Black = 0
        Line_Count = Line_Count + 1
    End If
End Sub

Sub M2_R3()
    ''Special case for Maze 2 room 3
    ''if we have crossed 3 lines, we run this

    ''adjust so we are aligned
    Turn_Anti_Clockwise
    Wait 50 ms 

    'Move forward until we are within 35 of the wall
    Move_Forward

    Do Forever
        Read_Sensors

        If Wall_Front_Distance < 35 Then
            ''once we are close enough to the wall, turn left 90 and proceed until there is a wall in front of us agin
            Move_Stop
            Wait 300 ms

            
            Turn_Anti_Clockwise
            Wait 305 ms 
            Turn_Clockwise
            Wait 80 ms

            Move_Stop
            Wait 300 ms


            'Move forward until wall < 50
            Move_Forward
            Do Forever
                Read_Sensors

                If Wall_Front_Distance < 50 Then 
                    ''The wall is close enough. Inch forward very slowly until we see the line now

                    ''break for hard stop - we dont want any risk of falling in the pit
                    Adjust_Left
                    Move_Backward
                    Wait 200 ms


                    ''repeatedly check for line while inchign forward
                    Do Forever 
                        Move_Forward
                        Wait 50 ms
                        Turn_Anti_Clockwise ''adjust for our bot naturally drifting
                        Wait 4 ms
                        Move_Stop
                        Wait 150 ms

                        ''Once we reach the line, stop and extinguish
                        If Line_Detect = 0 Then
                            Fan_On
                            Move_Stop
                            Wait 5 s

                            ''turn 45 in case we missed the flame
                            Turn_Anti_Clockwise
                            Wait 160 ms 
                            Move_Stop

                            Wait 5 s

                            ''turn 90 in case we still missed the flame
                            Turn_Clockwise
                            Wait 280 ms

                            Move_Stop
                            Wait 100 s
                        End If
                    Loop
                End If
            Loop
        End If
    Loop
End Sub

Sub M2_Flame()
    ''If a flame is decected to our right (flame in room 1 or 2)

    ''inch forward, stop, turn right, and inch forward again so we enter the room
    Move_Forward
    Wait 170 ms

    Move_Stop
    Wait 200 ms
    
    Turn_Right_90

    Move_Forward
    Wait 170 ms
    
    ''Turn the fan on - we are close enough to the flame
    Fan_On

    Do Forever
        Read_Sensors
    
        'Move forward until we are within 35 of front wall
        If Wall_Front_Distance > 35 Then
            Move_Forward

        Else
            'Spin Side to side to blow out fan
            Move_Stop
            Wait 5 s

            Turn_Anti_Clockwise
            Wait 140 ms 
            Move_Stop

            Wait 5 s

            Turn_Clockwise
            Wait 280 ms

            Move_Stop
            Wait 100 s
        End If
    Loop
End Sub

Sub M2_Wall_Follow()
    'decrement immunity (see Flame_Near for the use of immunity)
    If Immune > 0 Then
        Immune = Immune - 1
    End If

    Read_Sensors

    ''Execute room 3 code if we have crossed 3 lines 
    Check_Line
    If Line_Count > 2 Then
        Move_Stop
        Wait 1 s ''stop briefly so we know we counted 3 lines properly - this is purely for debugging purposes
        M2_R3
    End If

    'Handle flame in room 1/2
    If Flame_Value_2 < 10 Then
        M2_Flame
    End If

    'If none of the above cases, just left wall follow

    'Nothing to left - we want to turn left 
    If Wall_Left_Distance > 35 Then
        Move_Forward
        Wait 100 ms
        Move_Backward 'break for a hard stop
        Wait 60 ms
        Move_Stop
        Wait 100 ms

        Turn_Left_90
        Wait 100 ms

        'Diff from maze 1 follow - we need to scan for lines as we move this time
        Move_Forward
        ''repeat 550 times:
        For LoopCounter = 1 to 55
            For LoopCounter2 = 1 to 10
                Check_Line
                Wait 1 ms
            Next
        Next

        ''break for hard stop
        Move_Backward
        Wait 60 ms 
        Move_Stop

    'something to our left and something in front of us - turn right
    Else If (Wall_Front_Distance < 28) Then     
        Move_Backward 'break for a hard stop
        Wait 60 ms
        Move_Stop 

        Wait 300 ms

        Turn_Right_90
        Move_Stop
        Wait 100 ms

    ''follow a straight left wall if none of the above

    ''too close - adjust to be farther
    Else If Wall_Left_Distance < 12 Then
        Adjust_Right
        Move_Front_Right
    
    ''too far - adjust to be closer
    Else If Wall_Left_Distance > 18 Then
        Adjust_Left
        Move_Front_Left
    
    ''perfect - just move forward
    Else
        Move_Forward
    End If
End Sub

Sub Flame_Detect_M2()
    Read_Sensors
    
    ''Print flame values to find right thresholds
    CLS
    Locate 0,0
    Print Flame_Value_1
    Print ","
    Print Flame_Value_2
    Print ","
    Print Flame_Value_3

    Locate 1,0

    If Flame_Value_3 < 211 Then
        'Candle in Room 1
        Print "Room 1"
        Not_R4_Solve

    Else If Flame_Value_1 < 216 Then  
        'Candle in Room 2 (or 3)
        Print "Room 2/3"
        Not_R4_Solve
    
    Else    
        'Candle in 4 only or room 3 and 4
        Print "Room 3/4"
        M2_R4_Solve
    End If

    Not_R4_Solve 
End Sub

Sub Not_R4_Solve()
    ''turn left 90 and start wall following (we turn left since the bot starts facing forward)
    Move_Forward
    Wait 35 ms
    Move_Stop
    Wait 300 ms 
    
    Turn_Left_90

    Move_Stop

    Wait 300 ms

    Immune = 100
    
    Do Forever
        M2_Wall_Follow
    Loop
End Sub

Sub M2_R4_Solve()
    ''Flame is ether in room 4 or room 3 - assume its room 4 and check at the intersection
    ''if it's actually in room 3, backtrack and wall follow
    ''if not, head straight for room 4

    ''fan on signal (we send signal now because of 1 second delay; we turn off the signal so fan never turns on if not R4)
    Fan_On

    ''Move right while adjusting for natural bot drift
    Turn_Clockwise
    Wait 30 ms

    Move_Right
    Wait 400 ms 

    Turn_Clockwise
    Wait 20 ms 

    Move_Right
    Wait 400 ms
    
    Move_Right
    Wait 310 ms

    'break so we are stationary while reading for room 3
    Move_Left
    Wait 60 ms


    'now at first corner facing forward

    Read_Sensors

    If Flame_Value_1 < 150 Then
        'Room 3 is lit. Backtrack and wall follow
        Fan_Off
        Turn_Clockwise
        wait 15 ms
        Move_Left
        Wait 1400 ms
        Not_R4_Solve

    Else:
        ''Candle in room 4. Go R4 directly

        ''Adjust for drift and move forward
        Turn_Anti_Clockwise
        Wait 40 ms 
        
        Move_Forward
        Wait 400 ms
        Turn_Anti_Clockwise
        Wait 20 ms
        Move_Forward
        Wait 620 ms
        Move_Backward
        Wait 40 ms

        ' at the gap facing forward

        ''Turn 45 so our fan is in the right position to blow out the flame instantly
        Turn_Clockwise
        Wait 170 ms 
        Turn_Anti_Clockwise
        Wait 60 ms 

        'Move back left to get to R4 entrance
        Move_Back_Left
        Wait 800 ms

    
        ' at room 4 entrance

        'Move front left to get inside R4
        Move_Front_Left
        Wait 450 ms

        'Stop and blow out flame
        Move_Stop
        Wait 100 s
    End If
End Sub

'-------------------------MAIN-------------------------
Sub main() 
    If MAZE = 1 Then
        Fast_Room_4
        Do Forever
            M1_Left_Wall_Follow
        Loop
    Else If MAZE = 2 Then 
        Flame_Detect_M2
    End If

End Sub

MAZE = 2 
main
