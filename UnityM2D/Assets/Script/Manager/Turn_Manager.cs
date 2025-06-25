using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface ITurnParticipant
{
    void OnTurnStart();
    void OnTurnEnd();
    bool isAlive { get; } // 살아있는지 여부를 반환
}

public class TurnManager
{
    public enum TurnState
    {
        Ready, // 전투 준비 상태
        PlayerTurn,
        EnemyTurn,
        WaitingForAction,
        TurnEnd,
        CombatEnd // 전투 종료 상태
    }

    public TurnState CurrentTurnState { get; private set; }

    private List<ITurnParticipant> _turnOrder;
    private int _currentParticipantIndex;

    public TurnManager()
    {
        _turnOrder = new List<ITurnParticipant>();
        CurrentTurnState = TurnState.Ready; 
    }

    public void RegisterParticipant(ITurnParticipant participant)
    {
        if (!_turnOrder.Contains(participant)) 
        {
            _turnOrder.Add(participant);
        }
    }

    // 전투 시작
    public void StartCombat()
    {
        if (_turnOrder.Count < 2)
        {
            Debug.LogError("[TurnManager] 전투에 참여자가 부족합니다 (최소 플레이어1, 몬스터1).");
            CurrentTurnState = TurnState.CombatEnd;
            return;
        }

        _currentParticipantIndex = 0;
        CurrentTurnState = TurnState.Ready; 
        Debug.Log("[TurnManager] 전투 시작 준비!");
        StartNextTurn(); // 첫 턴 시작
    }

    // 다음 턴으로 진행
    public void StartNextTurn()
    {
        // 1. 전투 종료 조건 먼저 확인
        if (CheckCombatEndCondition())
        {
            EndCombat();
            return;
        }

        // 2. 현재 턴의 참여자 가져오기
        ITurnParticipant currentParticipant = GetCurrentActiveParticipant();

        if (currentParticipant == null) 
        {
            Debug.LogWarning("[TurnManager] 더 이상 행동할 참여자가 없습니다. 전투 종료.");
            EndCombat();
            return;
        }

        // 3. 턴 상태 설정
        if (currentParticipant is PlayerController)
        {
            CurrentTurnState = TurnState.PlayerTurn;
            Debug.Log("[TurnManager] 플레이어 턴!");
        }
        else if (currentParticipant is EnemyController)
        {
            CurrentTurnState = TurnState.EnemyTurn;
            Debug.Log("[TurnManager] 적 턴!");
        }
        else
        {
            currentParticipant.OnTurnEnd();
            EndCurrentTurn();
            return;
        }

        currentParticipant.OnTurnStart();

        CurrentTurnState = TurnState.WaitingForAction; 
    }

    public void EndCurrentTurn()
    {
        if (CurrentTurnState == TurnState.CombatEnd) 
            return;

        CurrentTurnState = TurnState.TurnEnd; 

        Debug.Log("[TurnManager] 턴 종료.");

        _currentParticipantIndex = (_currentParticipantIndex + 1) % _turnOrder.Count;

        StartNextTurn(); 
    }


    // 전투 종료 조건
    private bool CheckCombatEndCondition()
    {
        bool playerAlive = _turnOrder.Any(p => p is PlayerController && p.isAlive);
        bool anyEnemyAlive = _turnOrder.Any(p => p is EnemyController && p.isAlive);
        
        if (!playerAlive)
        {
            // Dead UI 
            Managers.TimerManager.StopTimer();
            Debug.Log("[TurnManager] 플레이어가 사망했습니다! 전투 패배.");
            
            return true;
        }

        if (!anyEnemyAlive)
        {
            Managers.TimerManager.StopTimer();
            Debug.Log("[TurnManager] 모든 적이 사망했습니다! 전투 승리.");
            return true; 
        }

        return false; // 아직 전투 중
    }

    private void EndCombat()
    {
        CurrentTurnState = TurnState.CombatEnd;
        Debug.Log("[TurnManager] --- 전투 종료! ---");

        // TODO : 필요한 경우, 전투 종료 후 UI 표시, 레벨 클리어 처리 등
    }

    // 다음 행동할 수 있는 참여자를 찾음
    private ITurnParticipant GetCurrentActiveParticipant()
    {
        int startIndex = _currentParticipantIndex;
        for (int i = 0; i < _turnOrder.Count; i++)
        {
            ITurnParticipant participant = _turnOrder[_currentParticipantIndex];
            if (participant.isAlive)
            {
                return participant; 
            }
            _currentParticipantIndex = (_currentParticipantIndex + 1) % _turnOrder.Count;
        }
        return null; 
    }
}