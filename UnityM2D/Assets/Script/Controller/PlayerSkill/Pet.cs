using System;
using System.Collections;
using UnityEngine;
using static Defines;


public class Pet : BaseController
{
    private CharacterManager<PetData> petDataManager = new CharacterManager<PetData>();
    public PetData petData => data as PetData;

    protected override ICharacterManager GetCharacterDataManager()
    {
        return petDataManager;
    }

    PetType petType = PetType.Slime;

    private float jumpDuration = 0.7f;
    private float jumpHeight = 2f;
    private float minRandomX = 0.5f;
    private float maxRandomX = 1.5f;
    private float jumpCooldown = 10f;

    private Vector3 spawnPosition;
    private Vector3 currentTargetPosition;

    private bool isJumping = false;
    private bool returnToSpawn = false;
    SpriteRenderer spriteRenderer = null;
    private void Start() => Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.Find(strPlayerObject);
        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
                this.transform.position = playerController.SettingAreaCollider();
        }
        myAnim = GetComponent<Animator>();

        GameObject spawner = Managers.Resource.Instantiate("Prefab/Pet/PetSpawner");
        if (spawner != null)
            spawnPosition = transform.position = spawner.transform.position;
        Destroy(spawner);

        if (petDataManager.Data == null)
            petDataManager.Data = new PetData();

        StartCoroutine(AutoJumpSequence());

        return _init = true;
    }

    private IEnumerator AutoJumpSequence()
    {
        while (true) // 무한 반복 점프 시퀀스
        {
            Vector3 startPos = transform.position;
            if (!returnToSpawn)
            {
                currentTargetPosition = startPos + new Vector3(UnityEngine.Random.Range(minRandomX, maxRandomX), 0, 0);
            }
            else
            {
                currentTargetPosition = spawnPosition;
            }

            yield return StartCoroutine(PerformJump(startPos, currentTargetPosition, jumpDuration));

            returnToSpawn = !returnToSpawn;

            yield return new WaitForSeconds(jumpCooldown);
        }
    }

    private IEnumerator PerformJump(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        isJumping = true; // 점프 시작 플래그
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // 시간 경과
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        transform.position = endPosition;
        isJumping = false; // 점프 종료 플래그
    }

    private void ChangePet(BaseController _owner)
    {
        if (data.Money > _owner.data.Money)
            return;

        if (petType >= PetType.EarthPet)
            return;

        _owner.data.Money -= data.Money;

        LoadData((PetType)((int)petType + 1));
    }

    public PetType UpgradePet(BaseController _owner)
    {
        ChangePet(_owner);

        PetType nextPetType = petType + 1;
        if(nextPetType >= PetType.EarthPet)
            nextPetType = PetType.EarthPet;
        return nextPetType;
    }


    private void LoadData(PetType _type)
    {
        if (petDataManager.Data == null)
            petDataManager.Data = new PetData();

        petData.petType = _type;
        petDataManager.ChangeData(petData);
        Sprite loadedSprite = Resources.Load<Sprite>(petData.prefab);

        if(loadedSprite != null)
        {
            petType = _type;

            spriteRenderer.sprite = loadedSprite; 
        }
    }
}
