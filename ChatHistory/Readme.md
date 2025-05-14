이 글은 **Microsoft.SemanticKernel의 `ChatHistory` 객체 사용법과 관리 방법**에 대해 설명하는 개발자 문서입니다. 아래에 핵심 내용을 정리하고, `AuthorName`의 사용 이유도 함께 설명드리겠습니다.

---

## ✅ 주요 내용 요약

### 1. **ChatHistory 객체란?**

* 대화 세션의 메시지를 저장하는 리스트 형태의 객체입니다.
* 사용자(User), 어시스턴트(Assistant), 시스템(System), 도구(Tool) 등의 메시지를 기록하여 **대화 흐름과 문맥을 유지**하는 데 사용됩니다.

---

### 2. **메시지 추가 방법**

* 간단한 방법:
  `chatHistory.AddUserMessage("질문")`
  
  `chatHistory.AddAssistantMessage("답변")`
  
  `chatHistory.AddSystemMessage("역할 설정")`

* 고급 방법:
  `ChatMessageContent` 객체를 직접 생성해서 `AuthorName`, 이미지, 함수 호출 등 **추가 정보**를 담을 수 있습니다.

---

### 3. **함수 호출 시뮬레이션**

* `FunctionCallContent`와 `FunctionResultContent`를 통해 툴 역할의 메시지를 추가할 수 있습니다.
* 예: 사용자의 알레르기 정보를 자동으로 불러오는 함수 호출을 시뮬레이션

```csharp
// 어시스턴트가 호출한 함수
FunctionCallContent(functionName: "get_user_allergies", pluginName: "User", id: "0001")

// 툴이 반환한 결과
FunctionResultContent(result: "{ \"allergies\": [\"peanuts\"] }", id: "0001")
```

💡 **Tip**: 툴 역할의 메시지에는 `id` 값이 반드시 있어야 합니다. 그래야 어떤 함수 호출에 대한 응답인지 매칭할 수 있습니다.

---

### 4. **ChatHistory 줄이기 (감소 전략)**

채팅이 길어질수록 메모리/성능/토큰 한계 등의 문제가 발생하므로 아래 방법으로 이력을 관리합니다.

#### 감축 전략

* **Truncation**: 오래된 메시지 삭제
* **Summarization**: 요약해서 한 줄로 압축
* **Token 기반 감축**: 토큰 수 기준으로 삭제 또는 요약

#### 예시:

```csharp
var reducer = new ChatHistoryTruncationReducer(targetCount: 2);
// 마지막 2개 메시지만 유지
```

---

### 5. **ChatHistory 감축 도구**

.NET에서 제공하는 대표적인 Reducer 2가지:

* `ChatHistoryTruncationReducer`: 오래된 메시지 삭제
* `ChatHistorySummarizationReducer`: 오래된 메시지 요약 후 다시 추가

---

## ❓ `AuthorName`은 왜 필요한가?

`AuthorName`은 **같은 역할(Role)** 내에서도 **개별 인물의 식별자**를 부여할 때 사용합니다.

예를 들어:

```csharp
chatHistory.Add(new() {
    Role = AuthorRole.User,
    AuthorName = "Ema Vargova",
    Content = "I'd like to have the first option, please."
});
```

* **User 역할이지만 여러 사람이 참여**하는 그룹 채팅 형태나 멀티 사용자 시나리오일 때 각 사용자를 구분할 수 있게 해줍니다.
* 또한, 시스템이나 어시스턴트 역할에서도 `AuthorName`을 설정해 **도메인/서비스 이름, 캐릭터 이름 등**을 명시할 수 있습니다.

---

## 📌 요약 정리

| 주제            | 설명                              |
| ------------- | ------------------------------- |
| ChatHistory란? | 대화 흐름을 저장하는 메시지 리스트             |
| 메시지 추가 방법     | 간단 추가 / 직접 객체 생성                |
| 함수 호출 시뮬레이션   | Tool 역할로 함수 호출 & 결과 전달 가능       |
| AuthorName 용도 | 동일 역할 내 개인/캐릭터 구분용              |
| 감축 필요성        | 성능, 토큰, 보안 등 이유로                |
| Reducer 도구    | Truncation, Summarization 방식 지원 |

---

필요하시면 예제 코드를 활용한 데모 코드나 구조도도 추가로 정리해드릴 수 있습니다.
