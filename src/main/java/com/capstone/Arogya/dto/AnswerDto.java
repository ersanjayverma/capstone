package com.capstone.Arogya.dto;

import com.capstone.Arogya.model.QuestionType;
import lombok.*;

import java.time.Instant;

@Data
@NoArgsConstructor 
@AllArgsConstructor 
@Builder
public class AnswerDto {
    private Long id;
    private Long userId;
    private Long questionId;
    private QuestionType questionType;
    private String answerText;
    private Integer answerInt;
    private Long selectedOptionId;
    private String selectedOptionText;
    private Instant answeredAt;
}
