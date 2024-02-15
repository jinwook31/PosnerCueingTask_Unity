using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TestTask : MonoBehaviour{
    public GameObject[] targets, endogenousCue;

    private MeshRenderer[] endogRender;
    private Outline[] exogenousCue;

    private int cueType = 0;  // 0: endo, 1: exo
    private int isValidType = 0;  // 0: non, 1: valid
    private int targetSide = -1;  // 0: left, 1: right

    private bool isResponsed = false, getResponse = false, isDisplayingCue = false;
    private bool setTarget = false;
    private int response = -1;

    private bool isResting = false;
    private int trialNum = 0;
    public int restTerm;
    private float responseTime;


    // Start is called before the first frame update
    void Awake(){
        
    }


    // Start is called before the first frame update
    void Start(){
        // Disable Mesh Renderer
        endogRender = new MeshRenderer[endogenousCue.Length];
        for(int i=0; i<endogenousCue.Length; i++){
            endogRender[i] = endogenousCue[i].GetComponent<MeshRenderer>();
            endogRender[i].enabled = false;
        }

        // Disable Outline
        exogenousCue = new Outline[targets.Length];
        for(int i=0; i<exogenousCue.Length; i++){
            exogenousCue[i] = targets[i].GetComponent<Outline>();
            exogenousCue[i].enabled = false;
        }

        TrialData.td.initTrialData();
    }


    // Update is called once per frame
    void Update(){

        if(isResting){
            if(Input.GetKeyDown(KeyCode.R))
                isResting=false;
            return;
        }
        
        // 2. 보여지는 동안에는 모든 인풋 무시
        if(isDisplayingCue == true){
            return;
        }


        // 3. 사용자 입력 대기
        if(isResponsed == false && getResponse == true){
            // set Targets (Only first time after cue)
            if(setTarget){
                setTarget = false;
                
                // Target 셋팅 - set Target Colors (red == target)
                if(targetSide == 0){
                    targets[0].GetComponent<Renderer>().material.color = Color.red;
                    targets[1].GetComponent<Renderer>().material.color = Color.green;
                }else{
                    targets[0].GetComponent<Renderer>().material.color = Color.green;
                    targets[1].GetComponent<Renderer>().material.color = Color.red;
                }

                Timer.timer.startTimer();
            }

            if(Input.GetKeyDown("left")){
                response = 0;
                isResponsed = true;
                responseTime = Timer.timer.stopTimer();
            }else if(Input.GetKeyDown("right")){
                response = 1;    
                isResponsed = true; 
                responseTime = Timer.timer.stopTimer();           
            }
        }


        // 4. 반응 후 결과 출력
        if(isResponsed == true){
            // 반응 후 결과 & Time log (추후 추가!)
            if(response == targetSide){
                Debug.Log("Correct!");
            }else{
                Debug.Log("Wrong!");
            }

            // save the trial result & get next trial line
            string[] trialResult = {Timer.timer.currentTime(), response.ToString(), responseTime.ToString()};
            trialNum++;
            bool readDone = TrialData.td.trialDone(false, trialResult);

            if(trialNum % restTerm == 0){
                // 쉬는 시간
                Debug.Log("REST SECTION!");
                isResting = true;
            }

            isResponsed = false;
            getResponse = false;
        }


        // 1. Trial 수행 & Cue 제시 및 관리
        if(getResponse == false && isDisplayingCue == false && !isResting){
            cueType = int.Parse(TrialData.td.getTrialInfo(2));
            isValidType = int.Parse(TrialData.td.getTrialInfo(3));
            targetSide = int.Parse(TrialData.td.getTrialInfo(4));

            // 첫 세팅 - Random 상태
            //cueType = Random.Range(0, 2);
            //isValidType = Random.Range(0, 2);
            //targetSide = Random.Range(0, 2);

            targets[0].GetComponent<Renderer>().material.color = Color.white;
            targets[1].GetComponent<Renderer>().material.color = Color.white;
            
            // Display & Delay for Cue
            StartCoroutine(presentCue());
        }
    }

    // 2. Cue 제시
    IEnumerator presentCue(){
        isDisplayingCue = true;

        yield return new WaitForSeconds(1.0f);  // ITI
        
        int cueSide = targetSide;
        if(isValidType == 0){  // Not valid cue: 반대로 변환
            if(cueSide == 0) cueSide = 1;
            else cueSide = 0;
        }

        // Present Cue
        if(cueType == 0){
            endogRender[cueSide].enabled = true;
        }else{
            exogenousCue[cueSide].enabled = true;
        }

        yield return new WaitForSeconds(1f);  // Delay

        // Disable Cue
        for(int i=0; i < exogenousCue.Length; i++){
            endogRender[i].enabled = false;
            exogenousCue[i].enabled = false;
        }

        yield return new WaitForSeconds(1f);  // Delay

        setTarget = true;
        isDisplayingCue = false;
        getResponse = true;
    }
}

