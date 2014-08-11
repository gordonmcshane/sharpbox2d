/*******************************************************************************
 * Copyright (c) 2013, Daniel Murphy
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 	* Redistributions of source code must retain the above copyright notice,
 * 	  this list of conditions and the following disclaimer.
 * 	* Redistributions in binary form must reproduce the above copyright notice,
 * 	  this list of conditions and the following disclaimer in the documentation
 * 	  and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
using SharpBox2D.Common;
using SharpBox2D.Dynamics;

namespace SharpBox2D.TestBed.Framework
{
    /**
 * Whether the setting effects the engine's behavior or
 * modifies drawing.
 *
 */
    public enum SettingType
    {
        DRAWING, ENGINE
    }

    /**
     * The type of value this setting pertains to
     */
    public enum ConstraintType
    {
        BOOLEAN, RANGE
    }
  
/**
 * Defines a setting used in the testbed.
 * @author Daniel Murphy
 */
public class TestbedSetting {
  

  public  string name;
  public  SettingType settingsType;
  public  ConstraintType constraintType;
  public bool enabled;
  public int value;
  public  int min;
  public  int max;
  
  public TestbedSetting(string argName, SettingType argType, bool argValue){
    name = argName;
    settingsType = argType;
    enabled = argValue;
    constraintType = ConstraintType.BOOLEAN;
    min = max = value = 0;
  }
  
  public TestbedSetting(string argName, SettingType argType, int argValue, int argMinimum, int argMaximum){
    name = argName;
    settingsType = argType;
    value = argValue;
    min = argMinimum;
    max = argMaximum;
    constraintType = ConstraintType.RANGE;
    enabled = false;
  }
}
}
