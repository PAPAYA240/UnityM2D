using System.Collections;
using UnityEngine;
using static Defines;


public class Pet : BaseController
{
    #region Private Fields

    private PetType _petType = PetType.Slime;
    public PetData MyPetData => data as PetData;
    private CharacterManager<PetData> _petDataManager = new CharacterManager<PetData>();

    private SpriteRenderer _spriteRenderer = null;

    #region Constants
    private const float JUMP_DURATION = 0.7f;
    private const float JUMP_HEIGHT = 2f;
    private const float JUMP_COOLDOWN = 10f;
    private const float MIN_RANDOM_X = 0.5f;
    private const float MAX_RANDOM_X = 1.5f;
    #endregion

    private Vector3 _spawnPosition;
    private Vector3 _destination;

    private bool _bJump = false;
    private bool _shouldReturnToSpawn = false;

    #endregion

    #region Initializer
    private void Awake() => Init();
  
    public override bool Init()
    {
        if (!base.Init())
            return false;

        if (!InitializeComponents())
            return false;

        InitializePosition();
        StartCoroutine(AutoJumpSequence());

        return _init = true;
    }
    private void InitializePosition()
    {
        GameObject playerObj = GameObject.Find(strPlayerObject);
        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
                transform.position = playerController.SettingAreaCollider();
        }

        // 스포너 위치로 최종 설정
        GameObject spawner = Managers.Resource.Instantiate(strPetSpanwer);
        if (spawner != null)
        {
            _spawnPosition = transform.position = spawner.transform.position;
            Destroy(spawner);
        }
    }
    private bool InitializeComponents()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _myAnimation = GetComponent<Animator>();

        if (_petDataManager.Data == null)
        {
            _petDataManager.Data = new PetData();
        }

        return (_spriteRenderer != null && _myAnimation != null);
    }
    protected override ICharacterManager GetCharacterDataManager()
    {
        return _petDataManager;
    }
    #endregion

    #region Jump Animation
    private IEnumerator AutoJumpSequence()
    {
        // 무한 반복 점프 시퀀스
        while (true)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = GetNextJumpTarget(startPos);

            yield return StartCoroutine(PerformJump(startPos, targetPos, JUMP_DURATION));

            _shouldReturnToSpawn = !_shouldReturnToSpawn;
            yield return new WaitForSeconds(JUMP_COOLDOWN);
        }
    }
    private Vector3 GetNextJumpTarget(Vector3 currentPos)
    {
        if (_shouldReturnToSpawn)
            return _spawnPosition;

        float randomX = Random.Range(MIN_RANDOM_X, MAX_RANDOM_X);
        return currentPos + new Vector3(randomX, 0, 0);
    }

    private IEnumerator PerformJump(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        _bJump = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        _bJump = false;
    }
    #endregion

    #region Upgrade Pet
    public PetType UpgradePet(BaseController owner)
    {
        if (!CanUpgrade(owner))
            return _petType;

        ProcessUpgrade(owner);

        return _petType;
    }

    private bool CanUpgrade(BaseController owner)
    {
        PetData data = Managers.PetLoader.GetPetDataByType(_petType);
        if (data == null)
            return false;
        if (data.price > owner.data.Money)
        {
            Debug.Log("펫 살 돈이 없음");
            return false;
        }

        if (_petType >= PetType.EarthPet)
        {
            Debug.Log("이미 FULL Level 임");
            return false;
        }
        return true;
    }

    private void ProcessUpgrade(BaseController owner)
    {
        PetData data = Managers.PetLoader.GetPetDataByType(_petType);
        owner.data.Money -= data.price;
            Debug.Log($"펫 값으로 {data.price} 사라짐");

        LoadPetData(_petType);
        _petType = _petType + 1;
    }
    #endregion

    #region Data Management
    private void LoadPetData(PetType type)
    {
        PetData data = Managers.PetLoader.GetPetDataByType(type);
        if (_petDataManager.Data == null)
            _petDataManager.Data = new PetData();

        _petDataManager.ChangeData(data);

        LoadPetSprite(data.petPrefab);
    }

    private void LoadPetSprite(string spritePath)
    {
        Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

        if (loadedSprite != null && _spriteRenderer != null)
            _spriteRenderer.sprite = loadedSprite;
    }

    #endregion
}
